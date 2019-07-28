# YukoBot
[![Discord Bots](https://discordbots.org/api/widget/status/594111117513719838.svg)](https://discordbots.org/bot/594111117513719838)

Yuko is a general purpose Discord bot that's main current feature is online and game notifications,
which Discord currently lacks. To add the bot to your server use [this link.](https://discordapp.com/oauth2/authorize?client_id=594111117513719838&scope=bot&permissions=8)

## Points
Points currently are gained based off of message activity with a current max point bonus of 10 per 10 minutes at 1 point per message.

## Notifications
Notifications are optional and are set with the **!notify _&lt;user&gt;_** and **!notify game _&lt;game&gt;_** commands.
When someone begins playing a game you're receiving notifications for you will only be notified if you're
also receiving notifications for that user.


You can think of it similar to how console notifications work where **!notify _&lt;user&gt;_** is adding them as a friend
and **!notify game _&lt;game&gt;_** is turning on notifications for a specific game. You wouldn't want to be notified every time
anyone on all of Xbox Live starts to play a game, just your friends.

## Commands
Here's a list of the current commands.
#### notifications
* !notify &lt;user&gt;
* !notify disable &lt;user&gt;
* !notify enable &lt;user&gt;
* !notify list 
* !notify remove &lt;user&gt;
* !notify game &lt;game&gt;
* !notify game remove &lt;game&gt;
* !notify game list

#### moderation
* !mute &lt;member&gt;
* !unmute &lt;member&gt;
* !ban &lt;member&gt;
* !ban &lt;member&gt; &lt;reason&gt;
* !kick &lt;member&gt;
* !kick &lt;member&gt; &lt;reason&gt;

#### points
* !points 
* !points &lt;user&gt;
* !points give &lt;user&gt; &lt;amount&gt;
* !points leaderboard

#### gambling
* !coin &lt;guess&gt; &lt;amount&gt;
* !slot &lt;amount&gt;

#### general
* !ping 
* !info
* !changelog
* !help 
* !help &lt;command&gt;

#### search
* !bing &lt;search&gt;
* !stackoverflow &lt;question&gt;
* !whybing

#### images
* !neko
* !kitsune
* !yuko

#### image processing
(include image attachment with message)
* !ascii
* !grayscale
