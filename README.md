# FeedbackService API Project
The new Feedback Service Project allows users to share feedback on their grocery orders, using the API endpoints to POST, GET, PUT, and DELETE their feedback.
Users are also allowed to share feedback on each of the items contained in their orders.

## System overview and key concepts
![System Overview](https://github.com/albertxcastro/FeedbackService/blob/master/Diagrams/SystemOverview.png)

As a high-level concept, the FeedbackService API is able to receive HTTP and it will read/store data, first, from the persistent storage service provided by Redis. 
If no data available there, it will be retrieved from an instance of PostgreSQL and the data will be cached using the Redis instance for future use.

### Key concepts
![Service Components](https://github.com/albertxcastro/FeedbackService/blob/master/Diagrams/ServiceComponents.png)

The design of the new components will consist of the following items:
* The Web API to expose the endpoints to rate orders and products, allowing the user to create, read, update and delete feedback.
* The Caching Manager which will reduce the number of hits to the database.
* The Data Access Layer, using a Facade implementation on top of a lightweight Entity Framework Core layer and Npgsql as data provider.

## Detail Design
### Web API
A set of RESTful endpoints will be exposed from the FeedbackService using ASP.NET Core, all of them requesting Basic Authentication and the userId (the customer who owns the order) in the headers.
* **OrderFeedback**: Used to rate existing orders. Methods:
  * **[GET]** /api/OrderFeedback/{orderId}: Gets the feedback of an existing order that matches the orderId parameter. The response is a JSON representing the Feedback (See data representation below).
    * If the order does not exist or has not been rated, or if the userId provided in the headers does not exist or does not own an order that matches the orderId, the action is aborted. Error messages displayed:
      * Order with Id {0} has not been rated. There is no feedback to retrieve.
      * Unable to retrieve order with orderId {0}
      * User {0} does not own an order with Id {1}

  * **[GET]** /api/OrderFeedback/GetLatest/{rating}:Asynchronously gets the last 20 order's feedback left by users filtered by the given rating numeric value, 
    in descendant order based on its creation time. If no rating is provided, the last 20 orders' feedback will be retrieved without filtering. 
    The response is a list of Feedback objects.
      * If the rating is not between 1 and 5, the action is aborted. Error message displayed:
        * Invalid rating. The rating must be between 1 to 5.

  * **[POST]** /api/OrderFeedback/{orderId}: Asynchronously posts feedback on an existing order (must match orderId). The order must haven't been rated yet.
  The response is a Feedback object.
    * If the orderId used refers to an order that has already been rated (i.e. feedback exists) or if the user does not own the order, the action is aborted. Error messages displayed:
      * The order you are trying to rate has already been rated. Try modifying its feedback instead.
  
  * **[PUT]** /api/OrderFeedback/{orderId}: Asynchronously updates an order's feedback by the given orderId. The response is a Feedback object with the updated values.
    * If the order does not exist or has not been rated, or if the userId provided in the headers does not exist or does not own an order that matches the orderId, the action is aborted. 
      Also checks for valid rating. Error messages displayed:
      * Unable to retrieve order with orderId {0}
      * User {0} does not own an order with Id {1}
      
  * **[DELETE]** /api/OrderFeedback/{orderId}: Asynchronously deletes an order's feedback for an order that matches the given orderId. The response is a message notifying that the feedback was successfully deleted.
    * If the order does not exist or has not been rated, or if the userId provided in the headers does not exist or does not own an order that matches the orderId, the action is aborted. Error messages displayed:
      * The feedback you are trying to delete does not exists.
      
* **ProductFeedback**: Used to rate products within existing orders. Methods:
  * **[GET]** /api/ProductFeedback/{orderId}/{productId}: Gets the feedback of a product that matches the productId parameter, which must be part of an existing order that matches the orderId parameter. The response is a JSON representing the Feedback with the product(See data representation below).
    * If the order does not exist or has not been rated, or if the product does not exist, or if the userId provided in the headers does not exist or does not own an order that matches the orderId, the action is aborted. Error messages displayed:
      * Unable to retrieve products associated to orderId {0}
      * The product has not been rated

  * **[POST]** /api/ProductFeedback/{orderId}/{productId}: Asynchronously posts feedback on a product which is part of an existing order (must match orderId). The product must haven't been rated yet.
  The response is a Feedback object with the product.
    * If the orderId used refers to a product that has already been rated (i.e. feedback exists) or if the product does not belong the order, the action is aborted. Error messages displayed:
      * The product you are trying to rate has already been rated. Try modifying its feedback instead.
  
  * **[PUT]** /api/ProductFeedback/{orderId}/{productId}: Asynchronously updates a product's feedback by the given productId. The response is a Feedback object with the updated values.
    * If the order does not contain the product or the product has not been rated, or if the userId provided in the headers does not exist or does not own an order that matches the orderId, the action is aborted. Error messages displayed:
      * Unable to retrieve products associated to orderId {0}
      * User {0} does not own an order with Id {1}
      
  * **[DELETE]** /api/ProductFeedback/{orderId}/{productId}: Asynchronously deletes a product's feedback which is part of an order that matches the given orderId. The response is a message notifying that the feedback was successfully deleted.
    * If the order does not exist or has not been rated, or if the userId provided in the headers does not exist or does not own an order that matches the orderId, the action is aborted. Error messages displayed:
      * The feedback you are trying to delete does not exists.
      
  #### FeedbackObject
```json
  {
    "sid": 0,
    "rating": 0,
    "comment": "string",
    "createTime": "2020-07-10T03:34:25.030Z",
    "orderSid": 0,
    "customerSid": 0,
    "feedbackType": 0,
    "products": [
      {
        "sid": 0,
        "name": "string",
        "price": 0
      }
    ]
  }
```

### Caching Manager
To reduce the number of calls made to the database for entity information, a caching manager will be implemented within the .NET Core application. This implementation will be a generic approach where, 
as a standard practice, the caching manager will have the following characteristics:
  * To periodically refresh to retrieve entity information (10 mins default). This should allow for enough usage by the web api without frequent hits on the databases.
  * The cache expiry value shall be a configuration value found within the .NET Core application configuration file for the specific objects which are leveraging the cache.
  * Calls to the database will be limited when the entities cache object is retrieved for the first time or after the expiry has expired, and there is a need for entity data.

The caching manager is provided by a shared library called CachingManager. It will provide all the necessary interfaces for managing distributed caching mechanisms (Redis, In-Memory, SQL server) for each required entity. 
A distributed cache instance is injected into the cache using a built-in IoC container with DI supported through the lifetime of the client. 

The technical approach will use Redis to take advantage of its speed and variety of data structures that it supports. Besides that, its configuration is easy and does not require a license, since it's an open-source tool.

### Data Access
The data access layer will be responsible for:
  * Retrieve entity data to insert into cache as necessary.
  * Insert order/product data into the entity tables.
  * Retrieve user information for authentication purposes.

The technical approach will use Entity Framework Core and Npgsql as data providers. PostgreSQL will be used as a database engine.

#### Database Diagram
![Database Diagram](https://github.com/albertxcastro/FeedbackService/blob/master/Diagrams/databaseDiagram.png)

## API Documentation
Swagger is used to autogenerate the API documentation.
![Swagger UI](https://github.com/albertxcastro/FeedbackService/blob/master/Diagrams/Swagger%20UI.png)

Please read more at the [Swagger website](https://swagger.io/)

## Live demo
The application has been deployed to AWS for testing purposes. It is up and running [here](http://feedbackserviceapi-dev.us-west-2.elasticbeanstalk.com/swagger/index.html) 

To authenticate, use:
 * **Username:** superadmin
 * **Password:** superadmin
 
In the headers, include **UserId** and the value can be from **1** to **5** (all of them have orders with products).
This would be the URL to retrieve the feedback on order 151:
 * http://feedbackserviceapi-dev.us-west-2.elasticbeanstalk.com/api/OrderFeedback/151
 
This would be the URL to retrieve the last 20 feedbacks left by users on orders filtered by rating and ordered by creation time descending. If no rating provided, the list is not filtered by rating but just ordered by creation time. For example, this retrieves feedback rated as 4:
 * http://feedbackserviceapi-dev.us-west-2.elasticbeanstalk.com/api/OrderFeedback/GetLatest/4

This would be the URL to retrieve the feedback on the product with productId 1 which is contained in order 117:
 * http://feedbackserviceapi-dev.us-west-2.elasticbeanstalk.com/api/ProductFeedback/117/1
