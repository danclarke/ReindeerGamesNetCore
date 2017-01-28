# Reindeer Games

This code is a C# port of the Reindeer sample from: https://github.com/amzn/alexa-skills-kit-json. For better accuracy I found it best to add the following text answers to the answer list:

* One
* Two
* Three
* Four

## The Projects

## Alexa

Core functionality for handling Alexa skills.

### ReindeerGames.Alexa

The shared Alexa functionality, handles the communication with Alexa. But not specific hosting related implementation details.

Has an associated test project too.

### ReindeerGames.Alexa.Lambda

AWS Lambda service for the Alexa skill & Reindeer Games game.

### ReindeerGames.Alexa.AzureService

Azure Web Service for the Alexa skill & Reindeer Games game. Uses Application Insights for logging, so you will need to hook up to an Insights key. Alternatively, you can amend / create a new logger that logs somewhere else - maybe even nowhere. The default logging implementation is 'InsightsLogger'.

Full security has not yet been implemented on this service - beware!

## Game

The Reindeer Games game itself along with associated tests.