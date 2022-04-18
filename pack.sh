FOLDER=.pack/
dotnet pack RabbitHub/ -o $FOLDER
dotnet pack RabbitHub.DI/ -o $FOLDER
