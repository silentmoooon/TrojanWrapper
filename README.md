# TrojanWrapper
`TrojanWrapper`是一个trojan的包装工具,在trojan的基础上增加了设置开机自启(通过托盘图标设置),以及通过privoxy提供http代理。
- 使用
>将trojan.exe(必须为该文件名,且全小写)和相关配置文件放到TrojanWrapper.exe所在的目录下，然后启动TrojanWrapper.exe，其中http代理端口默认为8088，可在config.txt文件中的listen-address 0.0.0.0:8088处修改。
- 环境
> .net 4.6