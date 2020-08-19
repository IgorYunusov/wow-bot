# AmeisenEventHook

This class captures events from WoW via Lua, makes it output a json, parses it and fires methods that you registered.

* * *

## How does it work

First it will create a Frame in WoW that will listen for events and inserts them into a table. That table will later be parsed and converted into a JSON.

```Lua
abFrame = CreateFrame("FRAME", "AbotEventFrame")
abEventTable = {}

function abEventHandler(self, event, ...)
    table.insert(abEventTable, {time(), event, {...}}) 
end

if abFrame:GetScript("OnEvent") == nil then
    abFrame:SetScript("OnEvent", abEventHandler) 
end
```

<br>

The table will be converted to JSON with a piece of Lua code, after this code has been executed you're able to read the *abEventJson* via *GetLocalizedText* or whatever you want with it.

```Lua
abEventJson='['

for a,b in pairs(abEventTable)do 
    abEventJson=abEventJson..'{'
    for c,d in pairs(b)do 
        if type(d)=="table"then 
            abEventJson=abEventJson..'\"args\": ['
            for e,f in pairs(d)do 
                abEventJson=abEventJson..'\"'..f..'\"'
                if e<=table.getn(d)then 
                    abEventJson=abEventJson..','
                end 
            end;
            abEventJson=abEventJson..']}'
            if a<table.getn(abEventTable)then 
                abEventJson=abEventJson..','
            end 
        else 
            if type(d)=="string"then 
                abEventJson=abEventJson..'\"event\": \"'..d..'\",'
            else 
                abEventJson=abEventJson..'\"time\": \"'..d..'\",'
            end 
        end 
    end 
end

abEventJson=abEventJson..']'
abEventTable={}
```

```C#
abFrame:RegisterEvent("eventName")

abFrame:UnregisterEvent("eventName")
```