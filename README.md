# Robotic

Robotic is a simple library that parses `robots.txt` files.

## Usage

Parsing a file:
```fs
"robots.txt"
|> IO.File.ReadLines
|> Robotic.parse
|> printfn "%A"
```

You can also dynamically build a robots.txt structure using the `Parser`'s module
API:
```fs
Parser.init
|> Parser.userAgent "googlebot"
|> Parser.userAgent "telegrambot"
|> Parser.allow "/"
|> Parser.disallow "/forbidden"
|> Parser.crawlDelay 10
|> Parser.userAgent "slackbot"
|> Parser.allow "/messages"
|> Parser.disallow "/admin"
|> Parser.crawlDelay 1
|> Parser.sitemap "/my-sitemap.xml"
|> Parser.finish
|> printfn "%A"
```
