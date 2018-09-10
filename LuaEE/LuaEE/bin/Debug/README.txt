blah blah blah. :V

LuaEE aka programming your bot in Lua with some C# sprinkled on top.

Here's some things you should know before using this bot.

'bot.lua' is the file the bot will always execute and it will crash if it's missing.

oh also, you can exit the console using -exit or -quit and restart bot.lua using -restart (report if it doesn't work).

request if you want to use EEPhysics than BlockWorld and I'll replace it with enough votes (including capasha's!)

[Inside bot.lua]
Some stuffs are already explained in 'nlua.org'.

init() is the function the bot will always execute on console intialization.

I have a Block handler added in the C# application. Feel free to use it if you like! It's variable is called "BlockWorld"
For the messages, it's responding function name is
'on' + (The type, either it's first letter is capitalized or not)

e.g. onInit(), onSay()

Oh also, these are the only stuffs I added in to make it simple, the rest is up to you... literally.

[Send a message, like con.Send]
Bot:Send(type)
   :Send(type, args)

[Connect to]
Connect:ToServer(email, password) -- Connects to EE's PlayerIO Server
       :ToWorld(worldID) -- Connects to the world (ToServer() must be executed already!)
       :ToLobby() -- Connects to the lobby (ToServer() must be executed already!)


[BlockWorld]
BlockWorld:HandleMessage(msg) -- Just like EEPhysics. Just insert the onMessage's msg here.
	  :GetBlock(x, y, layer)
             :GetBlocksById(id) -- Returns a list! Report to me if NLua doesn't like that.
                      Layer(layer)
                      XPos(x)
                      YPos(y)
                      Region(startx, endx, starty, endy, (optional) layer)
                      Placer(userid)
                      Type(msgType) -- "b", "bc", "bs", "lb", ...
