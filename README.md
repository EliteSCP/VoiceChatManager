# VoiceChatManager
An SCP: SL plugin which allows to record players' voice chat and play custom audios globally or in the proximity of a specific position or player.

## Minimum requirements
[EXILED](https://github.com/Exiled-Team/EXILED/releases/tag/5.3.0) **5.3.0+**

[FFmpeg](https://www.ffmpeg.org/download.html) (Optional, only if you want to directly convert audios from the plugin)

## How to install

1. Extract **VoiceChatManager.tar.gz**.

2. Put **VoiceChatManager.Core.dll** inside `%appdata%\Plugins\dependencies` if you're on **Windows** or `~/.config/Plugins/dependencies` on **Linux**.

3. Put **VoiceChatManager.dll** inside `%appdata%\Plugins` if you're on **Windows** or `~/.config/Plugins` on **Linux**.

## Player commands
| Command | Description |
| --- | --- |
| .forbidvoicerecording | Type this command to forbid to be voice recorded for security reasons. |
| .acceptvoicerecording | Type this command to accept to be voice recorded for security reasons. |

## Remote Admin/Server console commands
| Command | Description | Arguments | Permission | Example |
| --- | --- | --- | --- | --- |
| vcm stop | Stops an audio file from playing. | **[Preset name/File name/File path/Audio ID]** | **voicechatmanager.stop** | **vcm stop 0** |
| vcm play | Plays an audio file on the intercom channel. | **[File alias/File path] [Volume (0-100)]** | **voicechatmanager.play** | **vcm play C:\AmongUsMainTheme.mp3 100**|
| vcm play | Plays an audio file on a specific channel | **[File alias/File path] [Volume (0-100)] [Channel name (SCP, Intercom, Proximity, Player, Ghost)]** | | **vcm play C:\AmongUsMainTheme.mp3 100 SCP** |
| vcm play | Plays an audio file in the proximity of a specific player | **[File alias/File path] [Volume (0-100)] player [Player ID/Player Name/Player]** | | **vcm play C:\Users\Example\AmongUsMainThemeBassBoosted.mp3 100 player iopietro** |
| vcmp play | Plays an audio file in the proximity of a specific position. | **[File alias/File path] [Volume (0-100)] proximity [X] [Y] [Z]** | | **vcm play C:\Users\Example\AmongUsMainThemeBassBoosted.mp3 100 proximity 100 -50 33** |
| vcm pause | Pause an audio file from playing on the intercom channel. | **[Preset name/File name/File path/Audio ID]** | **vcm.pause** | **vcm pause 0** |
| vcm pause | Pause an audio file from playing on a specific channel. | **[Preset name/File name/File path/Audio ID] [Channel name  (SCP, Intercom, Proximity, Player, Ghost)]** | **vcm.pause** | **vcm pause Amogus player** |
| vcm list presets | Gets the list of audio presets. | | **vcm.list.presets** | |
| vcm list clear | Clears the audios list. | | **vcm.list.clear** | |
| vcm list audio | Gets the list of playing/paused/stopped audios. | | **vcm.list.audio** | |

## Limitations
Only **one** custom audio can be played at the same time.
FFmpeg is very **CPU consuming** and it opens a process for every conversion it does, so make sure your server is good enough before enabling the audio converter, otherwise it could create some lag.

## WARNING!
Read your local law regarding voice recording before activating the voice chat recorder and make sure to set `is_compliant` config to true if your server is hosted in the EU or people from EU join your server.

Playing earrapes is against [VSR](https://scpslgame.com/Verified_server_rules.pdf) read it before using this plugin.

Inspired by [CommsHack](https://github.com/VirtualBrightPlayz/CommsHack).
