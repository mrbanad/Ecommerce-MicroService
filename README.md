This repository contains a .NET Core microservice-based solution for an Ecommerce application. The solution is designed to be scalable, resilient and easy to maintain. It is built using a microservice architecture and utilizes various technologies such as Docker, RabbitMQ and MongoDB.

Getting Started
To get started with this project, you will need to have the following installed on your machine:

Visual Studio 2019 or later
Docker Desktop
Cloning the Repository
To clone this repository to your local machine, run the following command:

bash
Copy code
git clone https://github.com/mrbanad/Ecommerce-MicroService.git
Running the Application
To run the application, follow the steps below:

Open the solution in Visual Studio.
Set the docker-compose project as the startup project.
Press F5 or click on the Debug button to start the application.
This will start the application and all its dependencies such as the database, MongoDb and RabbitMQ in Docker containers.

Architecture
The Ecommerce Microservice solution is designed using a microservice architecture. The solution consists of the following microservices:

Catalog.API: Provides functionality related to managing products in the catalog.
Basket.API: Provides functionality related to managing items in the customer's basket.
Ordering.API: Provides functionality related to managing customer orders.
Payment.API: Provides functionality related to processing payments.
WebMVC: A web application that provides a user interface for the Ecommerce application.
Each microservice is a standalone application that can be deployed independently. The communication between microservices is done using RabbitMQ.

Technologies Used
The Ecommerce Microservice solution utilizes the following technologies:

.NET Core 7
Docker
RabbitMQ
MongoDb

Contributing
Contributions to this project are welcome. If you find a bug or have a feature request, please open an issue on the repository. If you would like to contribute code, please fork the repository and submit a pull request.
