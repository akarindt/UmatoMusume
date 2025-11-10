# UmatoMusume
A Windows desktop application that assists players of Umamusume Pretty Derby by providing a real-time event tracker and choice assistant using OCR (Optical Character Recognition).

# Sections
- [UmatoMusume](#umatomusume)
- [Sections](#sections)
- [Disclaimer](#disclaimer)
- [How does it works ?](#how-does-it-works-)
- [Features](#features)
- [How to use this application ?](#how-to-use-this-application-)
  - [Installation](#installation)
  - [Set captures area for OCR](#set-captures-area-for-ocr)
  - [Download/Crawl new data](#downloadcrawl-new-data)
- [Build](#build)
- [Dependencies](#dependencies)
- [License](#license)
- [Acknowledgments](#acknowledgments)

# Disclaimer
- I can't guarantee you that using this tool will not get your account banned.  
- This application interacts with the game through screen capture and OCR, which may violate Cygames' Terms of Service.  
- Use it at your own risk.

# How does it works ?
The application uses OCR (Optical Character Recognition) to capture text directly from the game screen. Once the text is recognized, it compares the result with its built-in event database.
- When an event appears in the game, the app scans it in real-time.
- The recognized text is then matched against known events using string similarity (to handle OCR mistakes).
- After matching, the app displays the corresponding choices and their effects so players can make the best decision immediately.

# Features
| Feature              | Description                                                                 |
|-----------------------|-----------------------------------------------------------------------------|
| Options suggestion    | Show recommended choices and their effects when an event appears.           |
| Real-time OCR         | Capture game screen text automatically and detect events instantly.         |
| String similarity | Handle OCR mistakes by fuzzy matching against event database.               |

# How to use this application ?
## Installation
- Download the [latest release](https://github.com/akarindt/UmatoMusume/releases/latest).
- Extract the zip file to a folder of your choice.
- Run UmatoMusume.exe.
## Set captures area for OCR
- At the main menu, there are two capture buttons, [Capture event] & [Capture date/time].
- How to capture:
  - Simply click the [Capture event]/[Capture date/time] button.
  - Hold & drag the area that contains events/datetime.
  - Enjoy !
- Set area for events: [Watch the video](https://www.youtube.com/watch?v=QjwQ3tL6vHA)
- Set area for date/time: [Watch the video](https://www.youtube.com/watch?v=UFjU6cFelxo)
- The event box only display choices after you choose an uma from combobox.
## Download/Crawl new data
This now automaticly web scraping the data from [game8](https://game8.co/games/Umamusume-Pretty-Derby/archives/539612) to grab the events and uma`s name. Every data is cache every new Card Event is being trigger.
- ~This feature helps you to get the latest data from [gametora](https://gametora.com/umamusume). Basically, it just a web crawler.~
- ~At the main menu, when click the [Download data], a dialog will appear:~

## Choose OCR engine from config
- I gave you two options for OCR engine: PaddleOCR & RapidOCR.
	- PaddleOCR: More accurate than RapidOCR but uses more RAM (=< 600mb). (Default)
	- RapidOCR: Less accurate but uses less RAM (=< 300mb).
- Choose one that fits your PC.
- You should be fine with PaddleOCR if you have 8GB+ RAM.

# Build
- This project uses [.NET 9.0](https://dotnet.microsoft.com/en-us/download/dotnet/9.0) and can be built using Visual Studio or the .NET CLI.
```
git clone https://github.com/akarindt/UmatoMusume.git
cd UmatoMusume
./build.bat [target]
```

# Dependencies
- [Selenium](https://www.selenium.dev/)
- [Tesseract OCR](https://github.com/tesseract-ocr/tessdata_best) (Old versions)
- [RapidOCR](https://github.com/RapidAI/RapidOCR) (Newest versions)
- [PaddleOCR](https://github.com/PaddlePaddle/PaddleOCR) (Newest versions)
- [FuzzySharp](https://github.com/JakeBayer/FuzzySharp)
- [StringSimilarity.NET](https://github.com/feature23/StringSimilarity.NET)
# License
- This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.
# Acknowledgments
- Game data sourced from [GameTora](https://gametora.com/umamusume) and [Game8](https://game8.co/games/Umamusume-Pretty-Derby/archives/539612)
- Special thanks to the Umamusume community


