const fs = require('fs');
const textmate = require("vscode-textmate");
const oniguruma = require("oniguruma");
const htmlParser = require('node-html-parser');
const escapeHtml = require('escape-html');
const colorTokens = require('./colorTokens');
const { settings } = require('cluster');

function readFile(path) {
    return new Promise((resolve, reject) => {
        fs.readFile(path, (error, data) => error ? reject(error) : resolve(data));
    })
}

function writeFile(path, contents) {
    return new Promise((resolve, reject) => {
        fs.writeFile(path, contents, (error) => error ? reject(error) : resolve());
    })
}

// Create a registry that can create a grammar from a scope name.
const registry = new textmate.Registry({
    onigLib: Promise.resolve({
        createOnigScanner: (sources) => new oniguruma.OnigScanner(sources),
        createOnigString: (str) => new oniguruma.OnigString(str)
    }),
    loadGrammar: (scopeName) => {
        if (scopeName === 'source.cs') {
            return readFile('./csharp.tmLanguage.json').then(data => {
                let grammar = textmate.parseRawGrammar(data.toString(), ".json");
                return grammar;
            });
        }
        console.log(`Unknown scope name: ${scopeName}`);
        return null;
    }
});

registry.loadGrammar('source.cs').then(grammar => {
    updateFile("C:/workspace/RestModels/docs/docfx_project/_site/articles/request_flow/index.html", grammar);
});

async function updateFile(path, grammar) {
    let contents = (await readFile(path)).toString();
    let rootNode = htmlParser.parse(contents, { pre: true });
    let codeBlocks = rootNode.querySelectorAll("pre");
    let updated = 0;
    for (let i = 0; i < codeBlocks.length; i++) {
        let node = codeBlocks[i];
        let colorized = colorizeNode(node.text, grammar);
        if (colorized){
            node.set_content(colorized);
            updated++;
        }
    }
    if (updated === 0) return;
    
    let result = rootNode.toString();
    await writeFile(path, result);
    console.log(`Updated ${updated} codeblocks in ${path}`);
}
function colorizeNode(raw, grammar) {
    if (!raw.startsWith('<code class="lang-csharp">')) return null;
    raw = raw.substring(26);
    if (raw.endsWith('</code>')) raw = raw.substring(0, raw.length - 7);
    const text = raw.split('\n').map(l => l.replace(/\r/g, ""));

    let output = "";
    let ruleStack = textmate.INITIAL;
    for (let i = 0; i < text.length; i++) {
        const line = text[i];
        const lineTokens = grammar.tokenizeLine(line, ruleStack);
        for (let j = 0; j < lineTokens.tokens.length; j++) {
            const token = lineTokens.tokens[j];
          /*  console.log(` - token from ${token.startIndex} to ${token.endIndex} ` +
            `(${line.substring(token.startIndex, token.endIndex)}) ` +
            `with scopes ${token.scopes.join(', ')}`
            );*/
            let sub = line.substring(token.startIndex, token.endIndex);
            if (token.scopes.length == 0) output += sub;
            else {
                let settingsList = token.scopes.map(s => matchScope(s)).filter(s => !!s);
                let settings = settingsList.length > 0 ? settingsList[0] : { foreground: "black" };
                if (!settings.foreground) settings.foreground = 'black';

                let style =  `color:${settings.foreground};`;
                if (settings.fontStyle) {
                    if (settings.fontStyle === 'bold') style += 'font-weight:bold;';
                    else if (settings.fontStyle === 'italic') style += 'font-style:italic';
                    else if (settings.fontStyle === 'underline') style += 'text-decoration:underline';
                }
                output += `<span style='${style}'>${escapeHtml(sub)}</span>`;
            }
        }
        output += '<br />';
        ruleStack = lineTokens.ruleStack;
    }

    return new htmlParser.TextNode(`<code class='lang-csharp'>${output}</code>`);
}

function matchScope(scope) {
    let parts = scope.split(".");

    while (parts.length > 0) {
        let match = matchScopeExact(parts.join("."));
        if (match) return match;
        parts.pop();
    }

    return null;
}

function matchScopeExact(scope) {
    for (let i = 0; i < colorTokens.length; i++) {
        let tokenProps = colorTokens[i];
        if (typeof tokenProps.scope == "string") {
            if (tokenProps.scope === scope) return tokenProps.settings;
            else continue;
        }

        if (tokenProps.scope.some(s => s === scope)) return tokenProps.settings;
    }

    return null;
}