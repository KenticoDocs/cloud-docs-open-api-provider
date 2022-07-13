![master](https://github.com/KenticoDocs/kontent-docs-blob-provider/actions/workflows/master_kcd-open-api-provider-live-master.yml/badge.svg)
![develop](https://github.com/KenticoDocs/kontent-docs-blob-provider/actions/workflows/develop_kcd-open-api-provider-live-dev.yml/badge.svg)

# Kontent.ai Documentation - OpenAPI provider

Backend function for Kontent.ai Learn that uses [Kontent.ai](https://kontent.ai/) as a source of its content.

The service is responsible for providing Kontent.ai's API reference in the OpenAPI format which is then displayed at [Kontent.ai Learn](https://kontent.ai/learn/).

## Architecture

!['Kontent.ai Learn - Blob Provider'](https://github.com/Kontent-ai-Learn/kontent-ai-learn-web/wiki/images/blob-provider.png)

## Overview

1. This project is a .NET Azure Durable Functions application.
2. It reacts to HTTP requests.
3. The function gets a request with API reference codename and returns the API reference which is stored in the Orchestrator instance. If the instance doesn't exist, the service triggers [API preprocessor](https://github.com/Kontent-ai-Learn/kontent-ai-learn-reference-preprocessor) which causes that an API reference blob will be created. The blob is then fetched by the OpenAPI provider and returned to the caller of the service.

## Setup

### Prerequisites

1. Visual Studio 2019 with Azure Functions and Web Jobs Tools installed.
2. Subscription on MS Azure.

### Instructions

1. Clone the project repository and open it in Visual Studio.
2. Install all the necessary nugget packages.
3. Set the required keys.
4. Run the service locally in Visual Studio, or
5. Deploy the service to a new Azure Functions App instance in your Azure subscription.

### Required keys

* `Storage.ConnectionString` - The connection string for the Azure storage account
* `EventGrid.ReferenceRequested.Endpoint` - EventGrid topic endpoint used when web initiates API reference request
* `EventGrid.ReferenceRequested.Key` - EventGrid topic key used when web initiates API reference request
* `EventGrid.ReferenceUpdated.Endpoint` - EventGrid topic endpoint used when API reference is updated in Kontent.ai
* `EventGrid.ReferenceUpdated.Key` - EventGrid topic key used when API reference is updated in Kontent.ai

## How To Contribute

Feel free to open a new issue where you describe your proposed changes, or even create a new pull request from your branch with proposed changes.

## License

All the source codes are published under MIT license.
