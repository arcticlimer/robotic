namespace Library

open System

type RuleGroup =
    { crawlDelay: int option
      allows: string list // TODO: Check for allowance and use hashsets?
      disallows: string list }

    static member Default =
        { allows = List.empty
          disallows = List.empty
          crawlDelay = None }


type ParsingToken =
    { groups: Map<string, RuleGroup>
      currentAgents: string list
      currentRuleGroup: RuleGroup
      isNewRuleGroup: bool
      sitemaps: string list }

    static member Default =
        { sitemaps = List.Empty
          groups = Map.empty
          currentAgents = List.empty
          currentRuleGroup = RuleGroup.Default
          isNewRuleGroup = false }


type Robots =
    { groups: Map<string, RuleGroup>
      sitemaps: string list }


module Parser =
    let private addToMap token =
        fun acc agent -> Map.add agent token.currentRuleGroup acc

    let private addCurrentAgentsToGroup token =
        token.currentAgents
        |> List.fold (addToMap token) token.groups

    let userAgent userAgent token =
        match token.isNewRuleGroup with
        | true ->
            let groups = addCurrentAgentsToGroup token

            { token with
                groups = groups
                currentAgents = [ userAgent ]
                isNewRuleGroup = false }

        | false ->
            let agents = userAgent :: token.currentAgents
            { token with currentAgents = agents }

    let crawlDelay crawlDelay token =
        let newRoleGroup = { token.currentRuleGroup with crawlDelay = Some crawlDelay }

        { token with
            currentRuleGroup = newRoleGroup
            isNewRuleGroup = true }

    let addSitemap right token =
        { token with
            sitemaps = right :: token.sitemaps
            isNewRuleGroup = true }

    let allow right token =
        let lst = right :: token.currentRuleGroup.allows
        let roleGroup = { token.currentRuleGroup with allows = lst }

        { token with
            currentRuleGroup = roleGroup
            isNewRuleGroup = true }

    let disallow right token =
        let lst = right :: token.currentRuleGroup.disallows
        let roleGroup = { token.currentRuleGroup with disallows = lst }

        { token with
            currentRuleGroup = roleGroup
            isNewRuleGroup = true }

    let finish token =
        let groups = addCurrentAgentsToGroup token

        { groups = groups
          sitemaps = token.sitemaps }

    let init = ParsingToken.Default

module Robotic =
    let private notHash c = not (c.Equals("#"))

    let private stripComments (line: string) =
        Seq.takeWhile notHash line |> String.Concat

    let private processLine (line: string) =
        match line.Split(":") with
        | [| cmd; value |] -> Some(cmd.ToLower(), stripComments (value.Trim()))
        | _ -> None

    let private folder token line =
        match line with
        | Some (directive, right) ->
            match directive with
            | "user-agent" -> Parser.userAgent right token
            | "crawl-delay" -> Parser.crawlDelay (int right) token
            | "allow" -> Parser.allow right token
            | "disallow" -> Parser.disallow right token
            | "sitemap" -> Parser.addSitemap right token
            | _ -> token
        | None -> token


    let parse (lines: string seq) =
        lines
        |> Seq.map processLine
        |> Seq.fold folder ParsingToken.Default
        |> Parser.finish

// TODO: Add module to check for allowance
