# 配置文件模板支持
## 之前，我是这样的
![image](http://images2017.cnblogs.com/blog/384997/201709/384997-20170921080044884-826579002.png)

因为公司ip和家里机器的ip不一致，所以经常需要切换配置文件。

但根据这份配置文件，我更改健康检查的主机和端口就意味着我得改三个地方，然而一般情况下这三个地方都是一致的，如果这时候我能定义一个变量“ServiceHost”，然后这三个地方使用这个变量就好了。
## 现在，我是这样的
![image](https://images2017.cnblogs.com/blog/384997/201709/384997-20170921080045696-1839914749.png)

如果有变更需要只需改动几个变量值就可以了，不需要在满屏的配置文件里面去查看、搜索替换了。
## 配置信息变更重新渲染
当配置文件变更，进行Reload时，模板会自动进行重新渲染，不用担心渲染之后配置监控不可用的问题。
## Samples
配置文件如下：  
![image](https://images2017.cnblogs.com/blog/384997/201709/384997-20170921080046181-200268300.png)  
代码如下：  
![image](https://images2017.cnblogs.com/blog/384997/201709/384997-20170921080046618-730649115.png)  
效果1（dotnet run）：  
![image](https://images2017.cnblogs.com/blog/384997/201709/384997-20170921080047009-2088195109.png)  
效果2（dotnet run --ServiceHost=localhost）：  
![image](https://images2017.cnblogs.com/blog/384997/201709/384997-20170921080047446-1509499016.png)  
效果3（dotnet run --ServiceHost=localhost --ServicePort=5000）：  
![image](https://images2017.cnblogs.com/blog/384997/201709/384997-20170921080048103-843199483.png)
# 基于 Consul 的 Configuration Provider
