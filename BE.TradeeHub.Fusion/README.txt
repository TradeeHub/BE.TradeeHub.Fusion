dotnet tool install --global HotChocolate.Fusion.CommandLine
dotnet new tool-manifest
dotnet tool install HotChocolate.Fusion.CommandLine

##Only needed for the fusion folder
dotnet new install HotChocolate.Templates

## this add the subgraph-config.json
dotnet fusion subgraph config set http --url http://localhost:5269 -w BE.TradeeHub.CustomerService.Application
dotnet fusion subgraph config set http --url http://localhost:5225 -w BE.TradeeHub.UserService

dotnet new graphql-gateway -n Gateway


This other guide might be better https://medium.com/workleap/the-only-local-mongodb-replica-set-with-docker-compose-guide-youll-ever-need-2f0b74dd8384

To created a mongodb set so you can use transaction since you need replicas here is link https://www.sohamkamani.com/docker/mongo-replica-set/
when link says  db = (new Mongo('localhost:27017')).getDB('test') the test is the database name I called it TradeeHub maybe should have been TradeeHubDB but oh well
var config = {
    "_id": "dbrs",
    "members": [
        {
            "_id": 0,
            "host": "mongo1:27017"
        },
        {
            "_id": 1,
            "host": "mongo2:27017"
        },
        {
            "_id": 2,
            "host": "mongo3:27017"
        }
    ]
};

rs.initiate(config);

when gpt and other stuff say mongo you need to change it to mongosh
to check the status run the below command
docker exec -it mongo1 mongosh --eval "rs.status()" 
the dbrs is the sets name you set in the docker compose 