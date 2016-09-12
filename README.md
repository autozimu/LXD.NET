# LXD.NET
[![Build status](https://ci.appveyor.com/api/projects/status/d9hk73a1opdlhxp9?svg=true)](https://ci.appveyor.com/project/JunfengLi/lxd-net)
[![NuGet version](https://badge.fury.io/nu/lxd.svg)](https://www.nuget.org/packages/LXD)

[LXD](http://www.ubuntu.com/cloud/lxd) client implemented in C#.

# Usage

This module is available as a [NuGet package](https://www.nuget.org/packages/LXD/). One can install it using NuGet Package Console window,

```
PM> Install-Package LXD
```

# Example

```CSharp
using LXD;

Client client = new Client(
    apiEndpoint: "https://your-lxd-service:8443",
    clientCertificateFilename: "your-client-certificate.p12",
    password: "your-client-certificate-password");

Console.WriteLine(client.Trusted); // true

foreach (Domain.Container container in client.Containers) {
    Console.WriteLine(container.Name);
}
// alpline
// ubuntu

Domain.Container alpine = client.Containers.First();
foreach (string str in alpine.Exec(new[] {"cat", "/etc/issue"})) {
    Console.WriteLine(st);
}
// Welcome to Alpine Linux 3.4
// Kernel \r on an \m (\l)
```
