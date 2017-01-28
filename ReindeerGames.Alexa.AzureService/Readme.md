# Reindeer Games - Azure Web Service

Execution of Reindeer Game for AI Assistants using Azure Web Services. I found it best to add the following text answers to the answer list when setting up the Alexa skill:

* One
* Two
* Three
* Four

## Azure Execution (HTTPS)

You can use the azurewebsites.net URL directly:

https://<service name>.azurewebsites.net/api/alexa/skill

In the SSL configuration page, set the certificate to "My development endpoint is a sub-domain of a domain that has a wildcard certificate from a certificate authority"

## Thanks

Special thanks to Tim Heuer for the excellent help using Alexa skills with .NET Core: http://timheuer.com/blog/archive/2016/12/12/amazon-alexa-skill-using-c-sharp-dotnet-core.aspx
