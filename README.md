# LuaEE
why melody

## Usage:
1. Create a folder containing a file called "main.lua".
2. Run the program

## Guide:
PlayerIO messages will call their respective function.

The name of the function is "`on` + it's respective name" (either with it's first letter capitalized or not).

For example: `onInit()` / `oninit()`, `onAdd()` / `onadd()`

## Included stuffs:
#### Connect.ToServer
```lua
Connect.ToServer("email","password",function(errorMsg)
    if errorMsg then
        -- Error! errorMsg doubles as the actual error message
        print('Error - ' .. errorMsg)
    else
        -- Successful Connection
    end
end)
```    
#### Connect.ToWorld
```lua
Connect.ToWorld("worldID",function(errorMsg)
    -- This functions the same as the one on .ToServer()
end)
```
#### Bot.Send
```lua
Bot.Send("m type")
```
#### Bot.SendArgs
```lua
Bot.SendArgs("m type",{arguments,here}) -- Take note of tostring and tonumber functions :>
```
#### Bot.Disconnect
```lua
Bot.Disconnect() -- Explains itself
```
#### trace
```lua
trace("Hello World!") -- Functions the same as print() (afaik, but im just used to using trace on NotITG)
```
#### onMsg
```lua
function onMsg(eType,e)
    -- This will be called no matter what message it is
    -- Useful for block handlers *wink wink*
end
```
#### onDisconnect
```lua
function onDisconnect(message)
    -- This will be called if the bot has disconnected.
end
```
#### BlockWorld
Check ``BlockWorld.cs``!
