# TouchSocket.Http

## 项目简介
TouchSocket.Http 是一个基于 Http1.1 协议的组件库，为开发者提供了丰富且强大的 Http 相关功能。它集成了 Http 服务器、客户端以及 WebSocket 组件，能够满足多种复杂场景下的网络通信需求。

## 功能特性
### 1. 数据传输功能
- **大文件处理**：支持大文件的下载与上传操作，并且实现了多线程下载和断点续传功能。这意味着在处理大文件时，不仅可以提高传输效率，还能在网络中断等异常情况下恢复传输，确保数据完整性。
- **小文件上传**：提供小文件 form 上传功能，方便用户通过表单形式上传小型文件。

### 2. WebApi 支持
- **声明与执行**：允许开发者声明和执行 WebApi，简化了与 Web 服务的交互过程，提高了开发效率。

### 3. 客户端特性
- **基于连接的客户端**：所提供的 Http 客户端是基于连接的，能够捕获连接和断开连接等消息。这使得开发者可以更好地监控客户端的状态，及时处理连接异常情况。

### 4. WebSocket 支持
- **WebSocket 组件**：集成了 WebSocket 组件，支持 WebSocket 协议，可实现实时双向通信，适用于需要实时数据交互的场景，如在线聊天、实时监控等。


## 支持的目标框架

- net481
- net462
- net472
- netstandard2.0
- netstandard2.1
- net6.0
- net9.0
- net8.0

## 使用方法

请参阅[说明文档(https://touchsocket.net/)](https://touchsocket.net/)