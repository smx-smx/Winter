cd %~dp0
java -jar swagger-codegen-cli-3.0.62.jar generate ^
-i http://localhost:5000/swagger/v1/swagger.json ^
-l typescript-axios ^
--model-name-prefix Winter ^
-o .\src\lib\api