How to build?

1)Open UserService.Api in Terminal

	dotnet build -o out .\UserService.Api.csproj
	docker build -t userservice .

2)Open ProductService.Api in Terminal

	dotnet build -o out .\ProductService.Api.csproj
 	docker build -t productservice .

3)Then

 	docker-compose up 
