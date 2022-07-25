namespace Library

open System

module RuleGroup =
    type T =
        { sitemap: string option // TODO: I think sitemap can also be a list
          crawlDelay: int option
          allows: string list
          disallows: string list }

    let init =
        { allows = List.empty
          disallows = List.empty
          crawlDelay = None
          sitemap = None }

module ParsingToken =
    type ParsingToken =
        { groups: Map<string, RuleGroup.T>
          currentAgents: string list
          currentRuleGroup: RuleGroup.T
          isNewRuleGroup: bool }

    let private addUserAgent (token: ParsingToken) (userAgent: string) : ParsingToken =
        match token.isNewRuleGroup with
        | true -> token
        // at the end of the rule group, set currentRuleGroup to each
        // named agent in the current group in the groups map.
        | false ->
            let agents = userAgent :: token.currentAgents
            { token with currentAgents = agents }

    let init =
        { groups = Map.empty
          currentAgents = List.empty
          currentRuleGroup = RuleGroup.init
          isNewRuleGroup = false }

    let folder token line =
        match line with
        | Some (directive, right) ->
            match directive with
            | "user-agent" -> addUserAgent token right
            | "crawl-delay" -> token
            | "allow" -> token
            | "disallow" -> token
            | "sitemap" -> token
            | _ -> token
        | None -> token


module Robotic =
    let private notHash c = not (c.Equals("#"))

    let private stripComments (line: string) =
        Seq.takeWhile notHash line |> String.Concat

    let private processLine (line: string) =
        match line.Split(":") with
        | [| cmd; value |] -> Some(cmd.ToLower(), stripComments (value.Trim()))
        | _ -> None

    let parse (lines: string list) =
        lines
        |> List.map processLine
        |> List.fold ParsingToken.folder ParsingToken.init
