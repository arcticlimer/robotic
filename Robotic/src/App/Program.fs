
open Library
open System

"/tmp/robots.txt"
|> IO.File.ReadLines
|> Robotic.parse
|> printfn "%A"

//Parser.init
//|> Parser.userAgent("codanado")
//|> Parser.userAgent("sapato")
//|> Parser.allow("/")
//|> Parser.disallow("/")
//|> Parser.crawlDelay(25)
//|> Parser.userAgent("foobar")
//|> Parser.allow("/testing")
//|> Parser.crawlDelay(100)
//|> Parser.finish
//|> printfn "%A"
