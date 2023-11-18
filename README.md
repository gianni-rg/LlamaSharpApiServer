# LlamaSharp API Server

This is an *experimental* .NET implementation of OpenAI-Compatible RESTful API, leveraging [LlamaSharp](https://github.com/SciSharp/LLamaSharp/) (.NET bindings of [llama.cpp](https://github.com/ggerganov/llama.cpp)).

- [LlamaSharp API Server](#llamasharp-api-server)
  - [Introduction](#introduction)
  - [Getting Started](#getting-started)
    - [Project Organization](#project-organization)
    - [Setup a local copy](#setup-a-local-copy)
  - [Contribution](#contribution)
  - [License](#license)

## Introduction

**It is an on-going work in progress, built in my spare time for fun & learning.**

The project has been mainly developed to host LLaMA 2 quantized models *locally*, and serving them using an OpenAI-Compatible RESTful API, to be consumed by [continue.dev](https://continue.dev/) Visual Studio Code extension (as a local OpenAI-compatible model). Please follow [this guide](https://continue.dev/docs/walkthroughs/codellama) to configure the extension.

It is a **ASP.NET Core (.NET 8.0) Minimal API project**.  
Currently it's a *very basic, bare-bone, very limited, but working implementation*.  
*It has bugs and can crash after some time that is in use*.

It supports quantized LLaMA 2 models made available by [TheBloke](https://huggingface.co/TheBloke). The following models have been tested:

- [llama-2-7b-chat-GGUF](https://huggingface.co/TheBloke/Llama-2-7B-Chat-GGUF) (Q4_K_S, Q5_K_S)
- [llama-2-13b-chat-GGUF](https://huggingface.co/TheBloke/Llama-2-13B-Chat-GGUF) (Q4_K_S, Q5_K_S)

You can download them from the provided links and store them in a local folder, which must be properly set in the app settings before starting the API server.

---

## Getting Started

### Project Organization

    ├── LICENSE
    ├── README.md                      <- The top-level README for developers using this project
    ├── docs                           <- Project documentation
    ├── src                            <- Source code
    │   ├── LlamaSharpApiServer        <- Main project
    |
    └── ...                            <- other files

### Setup a local copy

Clone the repository and build.  
You should be able to generate the application and run it.  
Configure server and model settings in `LlamaSharpApiServer/appconfig.json` file before running the server.

> When developing, DO NOT USE HTTPS, as otherwise continue.dev will not be able to validate the self-signed certificate and will not connect to the local APIs.

---

## Contribution

The project is constantly evolving and contributions are warmly welcomed.

I'm more than happy to receive any kind of contribution to this experimental project: from helpful feedbacks to bug reports, documentation, usage examples, feature requests, or directly code contribution for bug fixes and new and/or improved features.

Feel free to file issues and pull requests on the repository and I'll address them as much as I can, *with a best effort approach during my spare time*.

> Development is mainly done on Windows, so other platforms are not directly developed, tested or supported.  
> An help is kindly appreciated in make the application work on other platforms as well.

## License

You may find specific license information for third party software in the [third-party-programs.txt](./third-party-programs.txt) file.  
Where not otherwise specified, everything is licensed under the [APACHE 2.0 License](./LICENSE).

Copyright (C) 2023 Gianni Rosa Gallina.
