"use strict";(self.webpackChunktouchsocket=self.webpackChunktouchsocket||[]).push([[2424],{3905:(e,t,n)=>{n.d(t,{Zo:()=>p,kt:()=>m});var a=n(7294);function r(e,t,n){return t in e?Object.defineProperty(e,t,{value:n,enumerable:!0,configurable:!0,writable:!0}):e[t]=n,e}function o(e,t){var n=Object.keys(e);if(Object.getOwnPropertySymbols){var a=Object.getOwnPropertySymbols(e);t&&(a=a.filter((function(t){return Object.getOwnPropertyDescriptor(e,t).enumerable}))),n.push.apply(n,a)}return n}function i(e){for(var t=1;t<arguments.length;t++){var n=null!=arguments[t]?arguments[t]:{};t%2?o(Object(n),!0).forEach((function(t){r(e,t,n[t])})):Object.getOwnPropertyDescriptors?Object.defineProperties(e,Object.getOwnPropertyDescriptors(n)):o(Object(n)).forEach((function(t){Object.defineProperty(e,t,Object.getOwnPropertyDescriptor(n,t))}))}return e}function l(e,t){if(null==e)return{};var n,a,r=function(e,t){if(null==e)return{};var n,a,r={},o=Object.keys(e);for(a=0;a<o.length;a++)n=o[a],t.indexOf(n)>=0||(r[n]=e[n]);return r}(e,t);if(Object.getOwnPropertySymbols){var o=Object.getOwnPropertySymbols(e);for(a=0;a<o.length;a++)n=o[a],t.indexOf(n)>=0||Object.prototype.propertyIsEnumerable.call(e,n)&&(r[n]=e[n])}return r}var s=a.createContext({}),c=function(e){var t=a.useContext(s),n=t;return e&&(n="function"==typeof e?e(t):i(i({},t),e)),n},p=function(e){var t=c(e.components);return a.createElement(s.Provider,{value:t},e.children)},u="mdxType",k={inlineCode:"code",wrapper:function(e){var t=e.children;return a.createElement(a.Fragment,{},t)}},d=a.forwardRef((function(e,t){var n=e.components,r=e.mdxType,o=e.originalType,s=e.parentName,p=l(e,["components","mdxType","originalType","parentName"]),u=c(n),d=r,m=u["".concat(s,".").concat(d)]||u[d]||k[d]||o;return n?a.createElement(m,i(i({ref:t},p),{},{components:n})):a.createElement(m,i({ref:t},p))}));function m(e,t){var n=arguments,r=t&&t.mdxType;if("string"==typeof e||r){var o=n.length,i=new Array(o);i[0]=d;var l={};for(var s in t)hasOwnProperty.call(t,s)&&(l[s]=t[s]);l.originalType=e,l[u]="string"==typeof e?e:r,i[1]=l;for(var c=2;c<o;c++)i[c]=n[c];return a.createElement.apply(null,i)}return a.createElement.apply(null,n)}d.displayName="MDXCreateElement"},1759:(e,t,n)=>{n.r(t),n.d(t,{assets:()=>s,contentTitle:()=>i,default:()=>k,frontMatter:()=>o,metadata:()=>l,toc:()=>c});var a=n(7462),r=(n(7294),n(3905));const o={id:"websocketservice",title:"\u521b\u5efaWebSocket\u670d\u52a1\u5668"},i=void 0,l={unversionedId:"websocketservice",id:"websocketservice",title:"\u521b\u5efaWebSocket\u670d\u52a1\u5668",description:"\u5b9a\u4e49",source:"@site/docs/websocketservice.mdx",sourceDirName:".",slug:"/websocketservice",permalink:"/touchsocket/docs/current/websocketservice",draft:!1,editUrl:"https://gitee.com/rrqm_home/touchsocket/tree/master/handbook/docs/websocketservice.mdx",tags:[],version:"current",lastUpdatedBy:"\u82e5\u6c5d\u68cb\u8317",lastUpdatedAt:1702299732,formattedLastUpdatedAt:"Dec 11, 2023",frontMatter:{id:"websocketservice",title:"\u521b\u5efaWebSocket\u670d\u52a1\u5668"},sidebar:"docs",previous:{title:"\u4ea7\u54c1\u53ca\u67b6\u6784\u4ecb\u7ecd",permalink:"/touchsocket/docs/current/websocketdescription"},next:{title:"\u521b\u5efaWebSocket\u5ba2\u6237\u7aef",permalink:"/touchsocket/docs/current/websocketclient"}},s={},c=[{value:"\u5b9a\u4e49",id:"\u5b9a\u4e49",level:3},{value:"\u4e00\u3001\u8bf4\u660e",id:"\u4e00\u8bf4\u660e",level:2},{value:"\u4e8c\u3001\u53ef\u914d\u7f6e\u9879",id:"\u4e8c\u53ef\u914d\u7f6e\u9879",level:2},{value:"\u4e09\u3001\u652f\u6301\u63d2\u4ef6\u63a5\u53e3",id:"\u4e09\u652f\u6301\u63d2\u4ef6\u63a5\u53e3",level:2},{value:"\u56db\u3001\u521b\u5efaWebSocket\u670d\u52a1",id:"\u56db\u521b\u5efawebsocket\u670d\u52a1",level:2},{value:"4.1 \u7b80\u5355\u76f4\u63a5\u521b\u5efa",id:"41-\u7b80\u5355\u76f4\u63a5\u521b\u5efa",level:3},{value:"4.2 \u9a8c\u8bc1\u8fde\u63a5",id:"42-\u9a8c\u8bc1\u8fde\u63a5",level:3},{value:"4.3 \u901a\u8fc7WebApi\u521b\u5efa",id:"43-\u901a\u8fc7webapi\u521b\u5efa",level:3},{value:"4.4 \u901a\u8fc7Http\u4e0a\u4e0b\u6587\u76f4\u63a5\u521b\u5efa",id:"44-\u901a\u8fc7http\u4e0a\u4e0b\u6587\u76f4\u63a5\u521b\u5efa",level:3},{value:"4.5 \u521b\u5efa\u57fa\u4e8eSsl\u7684WebSocket\u670d\u52a1",id:"45-\u521b\u5efa\u57fa\u4e8essl\u7684websocket\u670d\u52a1",level:3},{value:"\u4e94\u3001\u63a5\u6536\u6d88\u606f",id:"\u4e94\u63a5\u6536\u6d88\u606f",level:2},{value:"5.1 \u63d2\u4ef6\u63a5\u6536\u6d88\u606f",id:"51-\u63d2\u4ef6\u63a5\u6536\u6d88\u606f",level:3},{value:"5.2 WebSocket\u663e\u5f0fReadAsync",id:"52-websocket\u663e\u5f0freadasync",level:3},{value:"\u516d\u3001\u56de\u590d\u3001\u54cd\u5e94\u6570\u636e",id:"\u516d\u56de\u590d\u54cd\u5e94\u6570\u636e",level:2},{value:"6.1 \u5982\u4f55\u83b7\u53d6SocketClient\uff1f",id:"61-\u5982\u4f55\u83b7\u53d6socketclient",level:3},{value:"\uff081\uff09\u76f4\u63a5\u83b7\u53d6\u6240\u6709\u5728\u7ebf\u5ba2\u6237\u7aef",id:"1\u76f4\u63a5\u83b7\u53d6\u6240\u6709\u5728\u7ebf\u5ba2\u6237\u7aef",level:4},{value:"\uff082\uff09\u901a\u8fc7Id\u83b7\u53d6",id:"2\u901a\u8fc7id\u83b7\u53d6",level:4},{value:"6.2 \u53d1\u9001\u6587\u672c\u7c7b\u6d88\u606f",id:"62-\u53d1\u9001\u6587\u672c\u7c7b\u6d88\u606f",level:3},{value:"6.3 \u53d1\u9001\u4e8c\u8fdb\u5236\u6d88\u606f",id:"63-\u53d1\u9001\u4e8c\u8fdb\u5236\u6d88\u606f",level:3},{value:"6.4 \u53d1\u9001\u5206\u5305\u7684\u4e8c\u8fdb\u5236",id:"64-\u53d1\u9001\u5206\u5305\u7684\u4e8c\u8fdb\u5236",level:3},{value:"6.5 \u76f4\u63a5\u53d1\u9001\u81ea\u5b9a\u4e49\u6784\u5efa\u7684\u6570\u636e\u5e27",id:"65-\u76f4\u63a5\u53d1\u9001\u81ea\u5b9a\u4e49\u6784\u5efa\u7684\u6570\u636e\u5e27",level:3},{value:"\u4e03\u3001\u670d\u52a1\u5668\u5e7f\u64ad\u53d1\u9001",id:"\u4e03\u670d\u52a1\u5668\u5e7f\u64ad\u53d1\u9001",level:2}],p={toc:c},u="wrapper";function k(e){let{components:t,...n}=e;return(0,r.kt)(u,(0,a.Z)({},p,n,{components:t,mdxType:"MDXLayout"}),(0,r.kt)("h3",{id:"\u5b9a\u4e49"},"\u5b9a\u4e49"),(0,r.kt)("p",null,"\u547d\u540d\u7a7a\u95f4\uff1aTouchSocket.Http.WebSockets ",(0,r.kt)("br",null),"\n\u7a0b\u5e8f\u96c6\uff1a",(0,r.kt)("a",{parentName:"p",href:"https://www.nuget.org/packages/TouchSocket.Http"},"TouchSocket.Http.dll")),(0,r.kt)("h2",{id:"\u4e00\u8bf4\u660e"},"\u4e00\u3001\u8bf4\u660e"),(0,r.kt)("p",null,"WebSocket\u662f\u57fa\u4e8eHttp\u534f\u8bae\u7684\u5347\u7ea7\u534f\u8bae\uff0c\u6240\u4ee5\u5e94\u5f53\u6302\u8f7d\u5728http\u670d\u52a1\u5668\u6267\u884c\u3002"),(0,r.kt)("h2",{id:"\u4e8c\u53ef\u914d\u7f6e\u9879"},"\u4e8c\u3001\u53ef\u914d\u7f6e\u9879"),(0,r.kt)("p",null,"\u7ee7\u627f",(0,r.kt)("a",{parentName:"p",href:"/touchsocket/docs/current/httpservice"},"HttpService")," "),(0,r.kt)("h2",{id:"\u4e09\u652f\u6301\u63d2\u4ef6\u63a5\u53e3"},"\u4e09\u3001\u652f\u6301\u63d2\u4ef6\u63a5\u53e3"),(0,r.kt)("table",null,(0,r.kt)("thead",{parentName:"table"},(0,r.kt)("tr",{parentName:"thead"},(0,r.kt)("th",{parentName:"tr",align:null},"\u63d2\u4ef6\u65b9\u6cd5"),(0,r.kt)("th",{parentName:"tr",align:null},"\u529f\u80fd"))),(0,r.kt)("tbody",{parentName:"table"},(0,r.kt)("tr",{parentName:"tbody"},(0,r.kt)("td",{parentName:"tr",align:null},"IWebSocketHandshakingPlugin"),(0,r.kt)("td",{parentName:"tr",align:null},"\u5f53\u6536\u5230\u63e1\u624b\u8bf7\u6c42\u4e4b\u524d\uff0c\u53ef\u4ee5\u8fdb\u884c\u8fde\u63a5\u9a8c\u8bc1\u7b49")),(0,r.kt)("tr",{parentName:"tbody"},(0,r.kt)("td",{parentName:"tr",align:null},"IWebSocketHandshakedPlugin"),(0,r.kt)("td",{parentName:"tr",align:null},"\u5f53\u6210\u529f\u63e1\u624b\u54cd\u5e94\u4e4b\u540e")),(0,r.kt)("tr",{parentName:"tbody"},(0,r.kt)("td",{parentName:"tr",align:null},"IWebSocketReceivedPlugin"),(0,r.kt)("td",{parentName:"tr",align:null},"\u5f53\u6536\u5230Websocket\u7684\u6570\u636e\u62a5\u6587")),(0,r.kt)("tr",{parentName:"tbody"},(0,r.kt)("td",{parentName:"tr",align:null},"IWebSocketClosingPlugin"),(0,r.kt)("td",{parentName:"tr",align:null},"\u5f53\u6536\u5230\u5173\u95ed\u8bf7\u6c42\u65f6\u89e6\u53d1\u3002\u5982\u679c\u5bf9\u65b9\u76f4\u63a5\u65ad\u5f00\u8fde\u63a5\uff0c\u5219\u6b64\u65b9\u6cd5\u5219\u4e0d\u4f1a\u89e6\u53d1\uff0c\u5c4a\u65f6\u53ef\u4ee5\u8003\u8651\u4f7f\u7528ITcpDisconnectedPlugin")))),(0,r.kt)("h2",{id:"\u56db\u521b\u5efawebsocket\u670d\u52a1"},"\u56db\u3001\u521b\u5efaWebSocket\u670d\u52a1"),(0,r.kt)("h3",{id:"41-\u7b80\u5355\u76f4\u63a5\u521b\u5efa"},"4.1 \u7b80\u5355\u76f4\u63a5\u521b\u5efa"),(0,r.kt)("p",null,"\u901a\u8fc7\u63d2\u4ef6\u521b\u5efa\u7684\u8bdd\uff0c\u53ea\u80fd\u6307\u5b9a\u4e00\u4e2a\u7279\u6b8aurl\u8def\u7531\u3002\u5982\u679c\u60f3\u83b7\u5f97\u8fde\u63a5\u524d\u7684Http\u8bf7\u6c42\uff0c\u4e5f\u5fc5\u987b\u518d\u6dfb\u52a0\u4e00\u4e2a\u5b9e\u73b0IWebSocketPlugin\u63a5\u53e3\u7684\u63d2\u4ef6\uff0c\u7136\u540e\u4eceOnHandshaking\u65b9\u6cd5\u4e2d\u6355\u83b7\u3002"),(0,r.kt)("pre",null,(0,r.kt)("code",{parentName:"pre",className:"language-csharp",metastring:"showLineNumbers",showLineNumbers:!0},'var service = new HttpService();\nservice.Setup(new TouchSocketConfig()//\u52a0\u8f7d\u914d\u7f6e\n    .SetListenIPHosts(7789)\n    .ConfigureContainer(a =>\n    {\n        a.AddConsoleLogger();\n    })\n    .ConfigurePlugins(a =>\n    {\n        a.UseWebSocket()//\u6dfb\u52a0WebSocket\u529f\u80fd\n        .SetWSUrl("/ws")//\u8bbe\u7f6eurl\u76f4\u63a5\u53ef\u4ee5\u8fde\u63a5\u3002\n        .UseAutoPong();//\u5f53\u6536\u5230ping\u62a5\u6587\u65f6\u81ea\u52a8\u56de\u5e94pong\n    }));\n\nservice.Start();\n\nservice.Logger.Info("\u670d\u52a1\u5668\u5df2\u542f\u52a8");\n')),(0,r.kt)("h3",{id:"42-\u9a8c\u8bc1\u8fde\u63a5"},"4.2 \u9a8c\u8bc1\u8fde\u63a5"),(0,r.kt)("p",null,"\u53ef\u4ee5\u5bf9\u8fde\u63a5\u7684Url\u3001Query\u3001Header\u7b49\u53c2\u6570\u8fdb\u884c\u9a8c\u8bc1\uff0c\u7136\u540e\u51b3\u5b9a\u662f\u5426\u6267\u884cWebSocket\u8fde\u63a5\u3002"),(0,r.kt)("pre",null,(0,r.kt)("code",{parentName:"pre",className:"language-csharp",metastring:"showLineNumbers",showLineNumbers:!0},'var service = new HttpService();\nservice.Setup(new TouchSocketConfig()//\u52a0\u8f7d\u914d\u7f6e\n    .SetListenIPHosts(7789)\n    .ConfigureContainer(a =>\n    {\n        a.AddConsoleLogger();\n    })\n    .ConfigurePlugins(a =>\n    {\n        a.UseWebSocket()//\u6dfb\u52a0WebSocket\u529f\u80fd\n               .SetVerifyConnection(VerifyConnection)\n               .UseAutoPong();//\u5f53\u6536\u5230ping\u62a5\u6587\u65f6\u81ea\u52a8\u56de\u5e94pong\n    }));\n\nservice.Start();\n\nservice.Logger.Info("\u670d\u52a1\u5668\u5df2\u542f\u52a8");\n')),(0,r.kt)("pre",null,(0,r.kt)("code",{parentName:"pre",className:"language-csharp",metastring:"showLineNumbers",showLineNumbers:!0},'/// <summary>\n/// \u9a8c\u8bc1websocket\u7684\u8fde\u63a5\n/// </summary>\n/// <param name="client"></param>\n/// <param name="context"></param>\n/// <returns></returns>\nprivate static bool VerifyConnection(IHttpSocketClient client, HttpContext context)\n{\n    if (!context.Request.IsUpgrade())//\u5982\u679c\u4e0d\u5305\u542b\u5347\u7ea7\u534f\u8bae\u7684header\uff0c\u5c31\u76f4\u63a5\u8fd4\u56defalse\u3002\n    {\n        return false;\n    }\n    if (context.Request.UrlEquals("/ws"))//\u4ee5\u6b64\u8fde\u63a5\uff0c\u5219\u76f4\u63a5\u53ef\u4ee5\u8fde\u63a5\n    {\n        return true;\n    }\n    else if (context.Request.UrlEquals("/wsquery"))//\u4ee5\u6b64\u8fde\u63a5\uff0c\u5219\u9700\u8981\u4f20\u5165token\u624d\u53ef\u4ee5\u8fde\u63a5\n    {\n        if (context.Request.Query.Get("token") == "123456")\n        {\n            return true;\n        }\n        else\n        {\n            context.Response\n                .SetStatus(403, "token\u4e0d\u6b63\u786e")\n                .Answer();\n        }\n    }\n    else if (context.Request.UrlEquals("/wsheader"))//\u4ee5\u6b64\u8fde\u63a5\uff0c\u5219\u9700\u8981\u4eceheader\u4f20\u5165token\u624d\u53ef\u4ee5\u8fde\u63a5\n    {\n        if (context.Request.Headers.Get("token") == "123456")\n        {\n            return true;\n        }\n        else\n        {\n            context.Response\n                .SetStatus(403, "token\u4e0d\u6b63\u786e")\n                .Answer();\n        }\n    }\n    return false;\n}\n')),(0,r.kt)("h3",{id:"43-\u901a\u8fc7webapi\u521b\u5efa"},"4.3 \u901a\u8fc7WebApi\u521b\u5efa"),(0,r.kt)("p",null,"\u901a\u8fc7WebApi\u7684\u65b9\u5f0f\u4f1a\u66f4\u52a0\u7075\u6d3b\uff0c\u4e5f\u80fd\u5f88\u65b9\u4fbf\u7684\u83b7\u5f97Http\u76f8\u5173\u53c2\u6570\u3002\u8fd8\u80fd\u5b9e\u73b0\u591a\u4e2aUrl\u7684\u8fde\u63a5\u8def\u7531\u3002\n\u5b9e\u73b0\u6b65\u9aa4\uff1a"),(0,r.kt)("ol",null,(0,r.kt)("li",{parentName:"ol"},"\u5fc5\u987b\u914d\u7f6eConfigureRpcStore\uff0c\u548c\u6ce8\u518cMyServer"),(0,r.kt)("li",{parentName:"ol"},"\u5fc5\u987b\u6dfb\u52a0WebApiParserPlugin")),(0,r.kt)("pre",null,(0,r.kt)("code",{parentName:"pre",className:"language-csharp",metastring:"showLineNumbers",showLineNumbers:!0},'var service = new HttpService();\nservice.Setup(new TouchSocketConfig()//\u52a0\u8f7d\u914d\u7f6e\n    .SetListenIPHosts(7789)\n    .ConfigureContainer(a =>\n    {\n        a.AddConsoleLogger();\n    })\n    .ConfigurePlugins(a =>\n    {\n        a.UseWebApi()\n        .ConfigureRpcStore(store =>\n        {\n            store.RegisterServer<MyServer>();\n        });\n    }));\n\nservice.Start();\n\nservice.Logger.Info("\u670d\u52a1\u5668\u5df2\u542f\u52a8");\n')),(0,r.kt)("pre",null,(0,r.kt)("code",{parentName:"pre",className:"language-csharp",metastring:"showLineNumbers",showLineNumbers:!0},'public class MyServer : RpcServer\n{\n    private readonly ILog m_logger;\n\n    public MyServer(ILog logger)\n    {\n        this.m_logger = logger;\n    }\n\n    [Router("/[api]/[action]")]\n    [WebApi(HttpMethodType.GET)]\n    public void ConnectWS(IWebApiCallContext callContext)\n    {\n        if (callContext.Caller is HttpSocketClient socketClient)\n        {\n            if (socketClient.SwitchProtocolToWebSocket(callContext.HttpContext))\n            {\n                m_logger.Info("WS\u901a\u8fc7WebApi\u8fde\u63a5");\n            }\n        }\n    }\n}\n')),(0,r.kt)("h3",{id:"44-\u901a\u8fc7http\u4e0a\u4e0b\u6587\u76f4\u63a5\u521b\u5efa"},"4.4 \u901a\u8fc7Http\u4e0a\u4e0b\u6587\u76f4\u63a5\u521b\u5efa"),(0,r.kt)("p",null,"\u4f7f\u7528\u4e0a\u4e0b\u6587\u76f4\u63a5\u521b\u5efa\u7684\u4f18\u70b9\u5728\u4e8e\u80fd\u66f4\u52a0\u4e2a\u6027\u5316\u7684\u5b9e\u73b0WebSocket\u7684\u8fde\u63a5\u3002"),(0,r.kt)("pre",null,(0,r.kt)("code",{parentName:"pre",className:"language-csharp",metastring:"showLineNumbers",showLineNumbers:!0},'class MyHttpPlugin : PluginBase, IHttpGetPlugin<IHttpSocketClient>\n{\n    public async Task OnHttpGet(IHttpSocketClient client, HttpContextEventArgs e)\n    {\n        if (e.Context.Request.UrlEquals("/GetSwitchToWebSocket"))\n        {\n            bool result = client.SwitchProtocolToWebSocket(e.Context);\n            return;\n        }\n        await e.InvokeNext();\n    }\n}\n')),(0,r.kt)("h3",{id:"45-\u521b\u5efa\u57fa\u4e8essl\u7684websocket\u670d\u52a1"},"4.5 \u521b\u5efa\u57fa\u4e8eSsl\u7684WebSocket\u670d\u52a1"),(0,r.kt)("p",null,"\u521b\u5efaWSs\u670d\u52a1\u5668\u65f6\uff0c\u5176\u4ed6\u914d\u7f6e\u4e0d\u53d8\uff0c\u53ea\u9700\u8981\u5728config\u4e2d\u914d\u7f6eSslOption\u4ee3\u7801\u5373\u53ef\uff0c\u653e\u7f6e\u4e86\u4e00\u4e2a\u81ea\u5236Ssl\u8bc1\u4e66\uff0c\u5bc6\u7801\u4e3a\u201cRRQMSocket\u201d\u4ee5\u4f9b\u6d4b\u8bd5\u3002\u4f7f\u7528\u914d\u7f6e\u975e\u5e38\u65b9\u4fbf\u3002"),(0,r.kt)("pre",null,(0,r.kt)("code",{parentName:"pre",className:"language-csharp",metastring:"showLineNumbers",showLineNumbers:!0},'var config = new TouchSocketConfig();\nconfig.SetServiceSslOption(new ServiceSslOption() //Ssl\u914d\u7f6e\uff0c\u5f53\u4e3anull\u7684\u65f6\u5019\uff0c\u76f8\u5f53\u4e8e\u521b\u5efa\u4e86ws\u670d\u52a1\u5668\uff0c\u5f53\u8d4b\u503c\u7684\u65f6\u5019\uff0c\u76f8\u5f53\u4e8ewss\u670d\u52a1\u5668\u3002\n      { \n          Certificate = new X509Certificate2("RRQMSocket.pfx", "RRQMSocket"), \n          SslProtocols = SslProtocols.Tls12 \n      });\n')),(0,r.kt)("h2",{id:"\u4e94\u63a5\u6536\u6d88\u606f"},"\u4e94\u3001\u63a5\u6536\u6d88\u606f"),(0,r.kt)("p",null,"WebSocket\u670d\u52a1\u5668\u63a5\u6536\u6d88\u606f\uff0c\u76ee\u524d\u6709\u4e24\u79cd\u65b9\u5f0f\u3002\u7b2c\u4e00\u79cd\u5c31\u662f\u901a\u8fc7\u8ba2\u9605",(0,r.kt)("inlineCode",{parentName:"p"},"IWebSocketReceivedPlugin"),"\u63d2\u4ef6\u5b8c\u5168\u5f02\u6b65\u7684\u63a5\u6536\u6d88\u606f\u3002\u7b2c\u4e8c\u79cd\u5c31\u662f\u8c03\u7528",(0,r.kt)("inlineCode",{parentName:"p"},"GetWebSocket"),"\u6269\u5c55\u65b9\u6cd5\u3002\u83b7\u53d6\u5230\u663e\u5f0f\u7684WebSocket\u5bf9\u8c61\uff0c\u7136\u540e\u8c03\u7528",(0,r.kt)("inlineCode",{parentName:"p"},"ReadAsync"),"\u65b9\u6cd5\u5f02\u6b65\u963b\u585e\u5f0f\u8bfb\u53d6\u3002"),(0,r.kt)("h3",{id:"51-\u63d2\u4ef6\u63a5\u6536\u6d88\u606f"},"5.1 \u63d2\u4ef6\u63a5\u6536\u6d88\u606f"),(0,r.kt)("p",null,"\u3010\u5b9a\u4e49\u63d2\u4ef6\u3011"),(0,r.kt)("pre",null,(0,r.kt)("code",{parentName:"pre",className:"language-csharp",metastring:"showLineNumbers",showLineNumbers:!0},'public class MyWebSocketPlugin : PluginBase, IWebSocketReceivedPlugin\n{\n    public async Task OnWebSocketReceived(IHttpClientBase client, WSDataFrameEventArgs e)\n    {\n        switch (e.DataFrame.Opcode)\n        {\n            case WSDataType.Cont:\n                client.Logger.Info($"\u6536\u5230\u4e2d\u95f4\u6570\u636e\uff0c\u957f\u5ea6\u4e3a\uff1a{e.DataFrame.PayloadLength}");\n\n                return;\n\n            case WSDataType.Text:\n                client.Logger.Info(e.DataFrame.ToText());\n\n                if (!client.IsClient)\n                {\n                    client.SendWithWS("\u6211\u5df2\u6536\u5230");\n                }\n                return;\n\n            case WSDataType.Binary:\n                if (e.DataFrame.FIN)\n                {\n                    client.Logger.Info($"\u6536\u5230\u4e8c\u8fdb\u5236\u6570\u636e\uff0c\u957f\u5ea6\u4e3a\uff1a{e.DataFrame.PayloadLength}");\n                }\n                else\n                {\n                    client.Logger.Info($"\u6536\u5230\u672a\u7ed3\u675f\u7684\u4e8c\u8fdb\u5236\u6570\u636e\uff0c\u957f\u5ea6\u4e3a\uff1a{e.DataFrame.PayloadLength}");\n                }\n                return;\n\n            case WSDataType.Close:\n                {\n                    client.Logger.Info("\u8fdc\u7a0b\u8bf7\u6c42\u65ad\u5f00");\n                    client.Close("\u65ad\u5f00");\n                }\n                return;\n\n            case WSDataType.Ping:\n                break;\n\n            case WSDataType.Pong:\n                break;\n\n            default:\n                break;\n        }\n\n        await e.InvokeNext();\n    }\n}\n')),(0,r.kt)("p",null,"\u3010\u4f7f\u7528\u3011"),(0,r.kt)("pre",null,(0,r.kt)("code",{parentName:"pre",className:"language-csharp",metastring:"{12}","{12}":!0},'var service = new HttpService();\nservice.Setup(new TouchSocketConfig()//\u52a0\u8f7d\u914d\u7f6e\n    .SetListenIPHosts(7789)\n    .ConfigureContainer(a =>\n    {\n        a.AddConsoleLogger();\n    })\n    .ConfigurePlugins(a =>\n    {\n        a.UseWebSocket()//\u6dfb\u52a0WebSocket\u529f\u80fd\n               .SetWSUrl("/ws");\n        a.Add<MyWebSocketPlugin>();//\u81ea\u5b9a\u4e49\u63d2\u4ef6\u3002\n    }));\n\nservice.Start();\n')),(0,r.kt)("admonition",{title:"\u63d0\u793a",type:"tip"},(0,r.kt)("p",{parentName:"admonition"},"\u63d2\u4ef6\u7684\u6240\u6709\u51fd\u6570\uff0c\u90fd\u662f\u53ef\u80fd\u88ab\u5e76\u53d1\u6267\u884c\u7684\uff0c\u6240\u4ee5\u5e94\u5f53\u505a\u597d\u7ebf\u7a0b\u5b89\u5168\u3002")),(0,r.kt)("h3",{id:"52-websocket\u663e\u5f0freadasync"},"5.2 WebSocket\u663e\u5f0fReadAsync"),(0,r.kt)("p",null,"WebSocket\u663e\u5f0fReadAsync\u6570\u636e\uff0c\u5b9e\u9645\u4e0a\u4e5f\u8981\u7528\u5230\u63d2\u4ef6\uff0c\u4f46\u662f\uff0c\u4f7f\u7528\u7684\u4ec5\u4ec5\u662f",(0,r.kt)("inlineCode",{parentName:"p"},"IWebSocketHandshakedPlugin"),"\uff0c\u56e0\u4e3a\u6211\u4eec\u53ea\u9700\u8981\u62e6\u622a",(0,r.kt)("inlineCode",{parentName:"p"},"\u63e1\u624b\u6210\u529f"),"\u7684\u6d88\u606f\u3002"),(0,r.kt)("pre",null,(0,r.kt)("code",{parentName:"pre",className:"language-csharp",metastring:"showLineNumbers",showLineNumbers:!0},'class MyReadWebSocketPlugin : PluginBase, IWebSocketHandshakedPlugin\n{\n    private readonly ILog m_logger;\n\n    public MyReadWebSocketPlugin(ILog logger)\n    {\n        this.m_logger = logger;\n    }\n    public async Task OnWebSocketHandshaked(IHttpClientBase client, HttpContextEventArgs e)\n    {\n        using (var websocket = client.GetWebSocket())\n        {\n            //\u6b64\u5904\u5373\u8868\u660ewebsocket\u5df2\u8fde\u63a5\n            while (true)\n            {\n                using (var receiveResult=await websocket.ReadAsync(CancellationToken.None))\n                {\n                    if (receiveResult.DataFrame==null)\n                    {\n                        break;\n                    }\n                    //\u5224\u65ad\u662f\u5426\u4e3a\u6700\u540e\u6570\u636e\n                    //\u4f8b\u5982\u53d1\u9001\u65b9\u53d1\u9001\u4e86\u4e00\u4e2a10Mb\u7684\u6570\u636e\uff0c\u63a5\u6536\u65f6\u53ef\u80fd\u4f1a\u591a\u6b21\u63a5\u6536\uff0c\u6240\u4ee5\u9700\u8981\u6b64\u5c5e\u6027\u5224\u65ad\u3002\n                    if (receiveResult.DataFrame.FIN)\n                    {\n                        if (receiveResult.DataFrame.IsText)\n                        {\n                            m_logger.Info($"WebSocket\u6587\u672c\uff1a{receiveResult.DataFrame.ToText()}");\n                        }\n                    }\n                }\n            }\n\n            //\u6b64\u5904\u5373\u8868\u660ewebsocket\u5df2\u65ad\u5f00\u8fde\u63a5\n            m_logger.Info("WebSocket\u65ad\u5f00\u8fde\u63a5");\n        }\n        await e.InvokeNext();\n    }\n}\n')),(0,r.kt)("p",null,"\u3010\u4f7f\u7528\u3011"),(0,r.kt)("pre",null,(0,r.kt)("code",{parentName:"pre",className:"language-csharp",metastring:"{16}","{16}":!0},'private static HttpService CreateHttpService()\n{\n    var service = new HttpService();\n    service.Setup(new TouchSocketConfig()//\u52a0\u8f7d\u914d\u7f6e\n        .SetListenIPHosts(7789)\n        .ConfigureContainer(a =>\n        {\n            a.AddConsoleLogger();\n        })\n        .ConfigurePlugins(a =>\n        {\n            a.UseWebSocket()//\u6dfb\u52a0WebSocket\u529f\u80fd\n              .SetWSUrl("/ws")//\u8bbe\u7f6eurl\u76f4\u63a5\u53ef\u4ee5\u8fde\u63a5\u3002\n              .UseAutoPong();//\u5f53\u6536\u5230ping\u62a5\u6587\u65f6\u81ea\u52a8\u56de\u5e94pong\n\n            a.Add<MyReadWebSocketPlugin>();\n        }));\n    \n    service.Start();\n\n    service.Logger.Info("\u670d\u52a1\u5668\u5df2\u542f\u52a8");\n    service.Logger.Info("\u76f4\u63a5\u8fde\u63a5\u5730\u5740=>ws://127.0.0.1:7789/ws");\n    return service;\n}\n')),(0,r.kt)("admonition",{title:"\u4fe1\u606f",type:"info"},(0,r.kt)("p",{parentName:"admonition"},(0,r.kt)("inlineCode",{parentName:"p"},"ReadAsync"),"\u7684\u65b9\u5f0f\u662f\u5c5e\u4e8e",(0,r.kt)("strong",{parentName:"p"},"\u540c\u6b65\u4e0d\u963b\u585e"),"\u7684\u63a5\u6536\u65b9\u5f0f\uff08\u548c\u5f53\u4e0bAspnetcore\u6a21\u5f0f\u4e00\u6837\uff09\u3002\u4ed6\u4e0d\u4f1a\u5355\u72ec\u5360\u7528\u7ebf\u7a0b\uff0c\u53ea\u4f1a\u963b\u585e\u5f53\u524d",(0,r.kt)("inlineCode",{parentName:"p"},"Task"),"\u3002\u6240\u4ee5\u53ef\u4ee5\u5927\u91cf\u4f7f\u7528\uff0c\u4e0d\u9700\u8981\u8003\u8651\u6027\u80fd\u95ee\u9898\u3002\u540c\u65f6\uff0c",(0,r.kt)("inlineCode",{parentName:"p"},"ReadAsync"),"\u7684\u597d\u5904\u5c31\u662f\u5355\u7ebf\u7a0b\u8bbf\u95ee\u4e0a\u4e0b\u6587\uff0c\u8fd9\u6837\u5728\u5904\u7406ws\u5206\u5305\u65f6\u662f\u975e\u5e38\u65b9\u4fbf\u7684\u3002")),(0,r.kt)("admonition",{title:"\u6ce8\u610f",type:"caution"},(0,r.kt)("p",{parentName:"admonition"},"\u4f7f\u7528\u8be5\u65b9\u5f0f\uff0c\u4f1a\u963b\u585e",(0,r.kt)("inlineCode",{parentName:"p"},"IWebSocketHandshakedPlugin"),"\u7684\u63d2\u4ef6\u4f20\u9012\u3002\u5e76\u4e14\u88ab\u8c03\u7528",(0,r.kt)("inlineCode",{parentName:"p"},"GetWebSocket"),"\u7684\u5ba2\u6237\u7aef\uff0c\u5728\u6536\u5230",(0,r.kt)("inlineCode",{parentName:"p"},"WebSocket"),"\u6d88\u606f\u7684\u65f6\u5019\uff0c\u4e0d\u4f1a\u518d\u89e6\u53d1\u63d2\u4ef6\u3002")),(0,r.kt)("h2",{id:"\u516d\u56de\u590d\u54cd\u5e94\u6570\u636e"},"\u516d\u3001\u56de\u590d\u3001\u54cd\u5e94\u6570\u636e"),(0,r.kt)("p",null,"\u8981\u56de\u590dWebsocket\u6d88\u606f\uff0c\u5fc5\u987b\u4f7f\u7528",(0,r.kt)("strong",{parentName:"p"},"HttpSocketClient"),"\u5bf9\u8c61\u3002"),(0,r.kt)("h3",{id:"61-\u5982\u4f55\u83b7\u53d6socketclient"},"6.1 \u5982\u4f55\u83b7\u53d6SocketClient\uff1f"),(0,r.kt)("h4",{id:"1\u76f4\u63a5\u83b7\u53d6\u6240\u6709\u5728\u7ebf\u5ba2\u6237\u7aef"},"\uff081\uff09\u76f4\u63a5\u83b7\u53d6\u6240\u6709\u5728\u7ebf\u5ba2\u6237\u7aef"),(0,r.kt)("p",null,"\u901a\u8fc7",(0,r.kt)("inlineCode",{parentName:"p"},"service.GetClients"),"\u65b9\u6cd5\uff0c\u83b7\u53d6\u5f53\u524d\u5728\u7ebf\u7684\u6240\u6709\u5ba2\u6237\u7aef\u3002"),(0,r.kt)("pre",null,(0,r.kt)("code",{parentName:"pre",className:"language-csharp",metastring:"showLineNumbers",showLineNumbers:!0},'HttpSocketClient[] socketClients = service.GetClients();\nforeach (var item in socketClients)\n{\n    if (item.Protocol == Protocol.WebSocket)//\u5148\u5224\u65ad\u662f\u4e0d\u662fwebsocket\u534f\u8bae\n    {\n        if (item.Id == "id")//\u518d\u6309\u6307\u5b9aid\u53d1\u9001\uff0c\u6216\u8005\u76f4\u63a5\u5e7f\u64ad\u53d1\u9001\n        {\n\n        }\n    }\n}\n')),(0,r.kt)("admonition",{title:"\u6ce8\u610f",type:"caution"},(0,r.kt)("p",{parentName:"admonition"},"\u7531\u4e8eHttpSocketClient\u7684\u751f\u547d\u5468\u671f\u662f\u7531\u6846\u67b6\u63a7\u5236\u7684\uff0c\u6240\u4ee5\u6700\u597d\u5c3d\u91cf\u4e0d\u8981\u76f4\u63a5\u5f15\u7528\u8be5\u5b9e\u4f8b\uff0c\u53ef\u4ee5\u5f15\u7528HttpSocketClient.Id\uff0c\u7136\u540e\u518d\u901a\u8fc7\u670d\u52a1\u5668\u67e5\u627e\u3002")),(0,r.kt)("h4",{id:"2\u901a\u8fc7id\u83b7\u53d6"},"\uff082\uff09\u901a\u8fc7Id\u83b7\u53d6"),(0,r.kt)("p",null,"\u5148\u8c03\u7528",(0,r.kt)("inlineCode",{parentName:"p"},"service.GetIds"),"\u65b9\u6cd5\uff0c\u83b7\u53d6\u5f53\u524d\u5728\u7ebf\u7684\u6240\u6709\u5ba2\u6237\u7aef\u7684Id\uff0c\u7136\u540e\u9009\u62e9\u9700\u8981\u7684Id\uff0c\u901a\u8fc7TryGetSocketClient\u65b9\u6cd5\uff0c\u83b7\u53d6\u5230\u60f3\u8981\u7684\u5ba2\u6237\u7aef\u3002"),(0,r.kt)("pre",null,(0,r.kt)("code",{parentName:"pre",className:"language-csharp",metastring:"showLineNumbers",showLineNumbers:!0},"string[] ids = service.GetIds();\nif (service.TryGetSocketClient(ids[0], out HttpSocketClient socketClient))\n{\n}\n")),(0,r.kt)("h3",{id:"62-\u53d1\u9001\u6587\u672c\u7c7b\u6d88\u606f"},"6.2 \u53d1\u9001\u6587\u672c\u7c7b\u6d88\u606f"),(0,r.kt)("pre",null,(0,r.kt)("code",{parentName:"pre",className:"language-csharp",metastring:"showLineNumbers",showLineNumbers:!0},'socketClient.SendWithWS("Text");\n')),(0,r.kt)("h3",{id:"63-\u53d1\u9001\u4e8c\u8fdb\u5236\u6d88\u606f"},"6.3 \u53d1\u9001\u4e8c\u8fdb\u5236\u6d88\u606f"),(0,r.kt)("pre",null,(0,r.kt)("code",{parentName:"pre",className:"language-csharp",metastring:"showLineNumbers",showLineNumbers:!0},"socketClient.SendWithWS(new byte[10]);\n")),(0,r.kt)("h3",{id:"64-\u53d1\u9001\u5206\u5305\u7684\u4e8c\u8fdb\u5236"},"6.4 \u53d1\u9001\u5206\u5305\u7684\u4e8c\u8fdb\u5236"),(0,r.kt)("p",null,"\u4f8b\u5982\uff1a\u53d1\u9001\u7684\u6570\u636e\u4e3a","[0,1,2,3,4,5,6,7,8,9]","\uff0c\u5f53\u8bbe\u7f6epackageSize\u4e3a5\u65f6\uff0c\u4f1a\u5148\u53d1\u9001","[0,1,2,3,4]","\u4f5c\u4e3a\u5934\u5305\uff0c\u7136\u540e\u53d1\u9001","[5,6,7,8,9]","\u7684\u540e\u7ee7\u5305\u3002"),(0,r.kt)("pre",null,(0,r.kt)("code",{parentName:"pre",className:"language-csharp",metastring:"showLineNumbers",showLineNumbers:!0},"byte[] data = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };\nsocketClient.SubSendWithWS(data, 5);\n")),(0,r.kt)("h3",{id:"65-\u76f4\u63a5\u53d1\u9001\u81ea\u5b9a\u4e49\u6784\u5efa\u7684\u6570\u636e\u5e27"},"6.5 \u76f4\u63a5\u53d1\u9001\u81ea\u5b9a\u4e49\u6784\u5efa\u7684\u6570\u636e\u5e27"),(0,r.kt)("pre",null,(0,r.kt)("code",{parentName:"pre",className:"language-csharp",metastring:"showLineNumbers",showLineNumbers:!0},'WSDataFrame frame=new WSDataFrame();\nframe.Opcode= WSDataType.Text;\nframe.FIN= true;\nframe.RSV1= true;\nframe.RSV2= true;\nframe.RSV3= true;\nframe.AppendText("I");\nframe.AppendText("Love");\nframe.AppendText("U");\nsocketClient.SendWithWS(frame);\n')),(0,r.kt)("admonition",{title:"\u5907\u6ce8",type:"info"},(0,r.kt)("p",{parentName:"admonition"},"\u6b64\u90e8\u5206\u529f\u80fd\u5c31\u9700\u8981\u4f60\u5bf9Websocket\u6709\u5145\u5206\u4e86\u89e3\u624d\u53ef\u4ee5\u64cd\u4f5c\u3002")),(0,r.kt)("admonition",{title:"\u6ce8\u610f",type:"caution"},(0,r.kt)("p",{parentName:"admonition"},"Websocket\u7684\u6240\u6709\u53d1\u9001\uff0c\u90fd\u662f\u5f62\u5982",(0,r.kt)("strong",{parentName:"p"},"SendWithWS"),"\u7684\u6269\u5c55\u65b9\u6cd5\u3002\u4e0d\u53ef\u76f4\u63a5Send\u3002")),(0,r.kt)("h2",{id:"\u4e03\u670d\u52a1\u5668\u5e7f\u64ad\u53d1\u9001"},"\u4e03\u3001\u670d\u52a1\u5668\u5e7f\u64ad\u53d1\u9001"),(0,r.kt)("pre",null,(0,r.kt)("code",{parentName:"pre",className:"language-csharp",metastring:"showLineNumbers",showLineNumbers:!0},'//\u5e7f\u64ad\u7ed9\u6240\u6709\u4eba\nforeach (var item in service.GetClients())\n{\n    if (item.Protocol== Protocol.WebSocket)\n    {\n        item.SendWithWS("\u5e7f\u64ad");\n    }\n}\n')),(0,r.kt)("admonition",{title:"\u63d0\u793a",type:"tip"},(0,r.kt)("p",{parentName:"admonition"},"\u5728\u53d1\u9001\u65f6\uff0c\u8fd8\u53ef\u4ee5\u81ea\u5df1\u8fc7\u6ee4Id\u3002")),(0,r.kt)("p",null,(0,r.kt)("a",{parentName:"p",href:"https://gitee.com/RRQM_Home/TouchSocket/tree/master/examples/WebSocket/WebSocketConsoleApp"},"\u672c\u6587\u793a\u4f8bDemo")))}k.isMDXComponent=!0}}]);