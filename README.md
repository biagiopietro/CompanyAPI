# CompanyAPI


Table of Contents
=================
* [Table of Contents](#table-of-contents)
  * [Company API](#company-api)
  * [Database configuration](#database-configuration)
  * [Seed](#seed)
  * [Testing](#testing)
     * [XUnit tests](#Xunit-tests)
  * [Docker](#docker)
     * [Build and Run](#build-and-run)
  * [Docker-compose](#docker-compose)
  * [Demo](#demo)
  * [License](#license)

## Company API

This is a simple ```Company API``` application written using ```C#```. These APIs allow you to *get*, *add*, *delete* and *update* employees and jobs. This application demostrates how it's simple to create an easy ```CRUD``` microservices using ```C#```. 

When the app starts up the a **swagger page** is shown and there you can test the APIs.

You can test the API exposed by the application using ```curl``` or ```Postman``` or whatever you want as well :)



## Database Configuration
As you can see the project contains these files:
 
-   ```appsettings.json``` (It should be used for the **production** environment);
-   ```appsettings.Development.json```;
-   ```appsettings.Docker.json```;
-   ```appsettings.Test.json```.

Each of the above files contains the json attribute ```ConnectionStrings.CompanyDB``` and here you have to setup the connection to you database. Each of ```appsettings.*.json``` (except appsettings.Docker.json) as default contains a connection to the local ```mysql``` database. Instead, the ```appsettings.Docker.json``` file contains the ```service name``` of the mysql container described into the ```docker-compose.yml``` (see [Docker-compose](#docker-compose) section).


## Seed
On start up of the project, it will automatically create a database with the name specified by the ```ConnectionStrings.CompanyDB``` attribute and seed it. Obviously the system gets the right appsettings.*.json file based on this ```env``` variable: ```ASPNETCORE_ENVIRONMENT``` (you can find it in ```.vscode/launch.json``` file).

**Note**

If you want to change the seeds please refer to the ```Seeds/``` folder.


## Testing

### XUnit tests

To be sure that the project work after every change that you make, please run the xunit tests. 

So if you use ```Visual Studio Code``` I suggest to you to install the **.NET COre Test Explorer by Jun Han** extentions because it gives to you a useful UI where you can see and run the tests.

Otherwise you can run the tests using the CLI so you have to simple run this command:

``` 
dotnet test
```
## Docker

### Build and Run
I provided a ```Dockerfile``` to allow you to build your own docker image as well. As you can see I've setted the ```ASPNETCORE_ENVIRONMENT``` env variable to ```Docker```; in this way the application will read the ```appsettings.Docker.json``` file. 
Before to build the application, you have to 'publish' your app so you have to run this command:

```
dotnet publish-c Release
```

In this way the system generates the binaries under the ```bin/Release``` folder.

Finally you can run:
```
docker build -t company-api
```
to generate the docker image.

## Docker-compose
You can run ```Company-API + MySQL``` using ```docker-compose```.

So move inside the Company-API folder and run ```docker-compose up -d``` and see the magic!

**TA TA TADAA !!!**

## Demo

![](https://imgur.com/a/XexnTNp.gif)


## License
   Copyright 2020 biagiopietro

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.