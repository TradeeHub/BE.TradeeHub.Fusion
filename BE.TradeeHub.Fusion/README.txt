dotnet tool install --global HotChocolate.Fusion.CommandLine
dotnet new tool-manifest
dotnet tool install HotChocolate.Fusion.CommandLine

##Only needed for the fusion folder
dotnet new install HotChocolate.Templates

## this add the subgraph-config.json
dotnet fusion subgraph config set http --url http://localhost:5269 -w BE.TradeeHub.CustomerService.Application
dotnet fusion subgraph config set http --url http://localhost:5225 -w BE.TradeeHub.UserService

dotnet new graphql-gateway -n Gateway