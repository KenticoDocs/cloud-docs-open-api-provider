| [master](https://github.com/KenticoDocs/cloud-docs-open-api-provider/tree/master) | [develop](https://github.com/KenticoDocs/cloud-docs-open-api-provider/tree/develop) |
|:---:|:---:|
| [![Build Status](https://travis-ci.com/KenticoDocs/cloud-docs-reference-provider.svg?branch=master)](https://travis-ci.com/KenticoDocs/cloud-docs-reference-provider/branches) [![codebeat badge](https://codebeat.co/badges/e24313b3-455f-4664-99c1-80d8670a7d73)](https://codebeat.co/projects/github-com-kenticodocs-cloud-docs-open-api-provider-master) | [![Build Status](https://travis-ci.com/KenticoDocs/cloud-docs-reference-provider.svg?branch=develop)](https://travis-ci.com/KenticoDocs/cloud-docs-reference-provider/branches) [![codebeat badge](https://codebeat.co/badges/c1471ae5-01f6-4ade-8a8d-677386740e9d)](https://codebeat.co/projects/github-com-kenticodocs-cloud-docs-open-api-provider-develop) |

# Kentico Cloud Documentation - OpenAPI provider

Backend function for Kentico Cloud documentation portal, which utilizes [Kentico Cloud](https://app.kenticocloud.com/) as a source of its content.

The service is responsible for providing Kentico Cloud's API reference in the OpenAPI format which is then displayed on the [Kentico Cloud Docs website](https://docs.kenticocloud.com/).

## Overview
1. This project is a TypeScript Azure Durable Functions application.
2. It reacts to HTTP requests.
3. The function fetches the API reference from [Azure Blob storage](https://docs.microsoft.com/en-us/azure/storage/blobs/storage-blobs-introduction), stores it in the orchestrator instance and returns it.

## Setup

### Prerequisites
1. Node (+yarn) installed
2. Visual Studio Code installed
3. Subscriptions on Kentico Cloud

### Instructions
1. Open Visual Studio Code and install the prerequisites according to the [following steps](https://code.visualstudio.com/tutorials/functions-extension/getting-started).
2. Log in to Azure using the Azure Functions extension tab.
3. Clone the project repository and open it in Visual Studio Code.
4. Run `yarn install` in the terminal.
5. Set the required keys.
6. Deploy to Azure using Azure Functions extension tab, or run locally by pressing `Ctrl + F5` in Visual Studio Code.

## Testing
* Run `yarn run test` in the terminal.

## How To Contribute
Feel free to open a new issue where you describe your proposed changes, or even create a new pull request from your branch with proposed changes.

## Licence
All the source codes are published under MIT licence.
