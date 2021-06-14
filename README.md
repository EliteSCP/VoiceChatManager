# VoiceChatManager
An SCP: SL plugin which permits to record players' voice chat and play custom audios globally or in the proximity of a specific position or player.

## Minimum requirements
[EXILED](https://github.com/Exiled-Team/EXILED/tags) **2.8.0+**

### Player commands
| Command | Description |
| --- | --- |
| .forbidvoicerecording | Type this command to forbid to be voice recorded for security reasons. |
| .acceptvoicerecording | Type this command to accept to be voice recorded for security reasons. |

### Remote Admin/Server console commands
| Command | Description | Arguments | Permission | Example |
| --- | --- | --- | --- | --- |
| vcm stop | Stops an audio file from playing. | **[Preset name/File name/File path/Audio ID]** | **voicechatmanager.stop** | **vcm stop 0** |
| vcm play | Plays an audio file on the intercom channel. | **[File alias/File path] [Volume (0-100)]** | **voicechatmanager.start** | **vcm play C:\AmongUsMainTheme.mp3 100**|
| vcm play | Plays an audio file on a specific channel | **[File alias/File path] [Volume (0-100)] [Channel name (SCP, Intercom, Proximity, Ghost)]** | // | **vcm play C:\AmongUsMainTheme.mp3 100 SCP** |
| vcm play | Plays an audio file in the proximity of a specific player | **[File alias/File path] [Volume (0-100)] proximity [Player ID/Player Name/Player]** | // | **vcm play C:\Users\Example\AmongUsMainThemeBassBoosted.mp3 100 proximity iopietro** |
| vcmp play | Plays an audio file in the proximity of a specific position. | **[File alias/File path] [Volume (0-100)] proximity [X] [Y] [Z]** | // | **vcm play C:\Users\Example\AmongUsMainThemeBassBoosted.mp3 100 proximity 100 -50 33** |
| vcm pause | Stops an audio file from playing. | **[Preset name/File name/File path/Audio ID]** | **voicechatmanager.pause** | **vcm pause 0**
| vcm list presets | Gets the list of audio presets. | | **voicechatmanager.list.presets** | |
| vcm list clear | Clears the audios list. | | **voicechatmanager.list.clear** | |
| vcm list audio | Gets the list of playing/paused/stopped audios. | | **voicechatmanager.list.audio** | |

## Limitations
Only **one** custom audio can be played at the same time.

## WARNING!
Read your local law regarding voice recording before activating the voice chat recorder and make sure to set `is_compliant` config to true if your server is hosted in the EU or people from EU join your server.