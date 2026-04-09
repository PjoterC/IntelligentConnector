# Public API Data Weaver - Random Facts From Cats (PL version below)

The program combines the functionalities of two APIs from the [public-apis](https://github.com/public-apis/public-apis) repository: 
* **Random Useless Facts** - https://uselessfacts.jsph.pl/ 
* **Cat as a Service (CATAAS)** - https://cataas.com/

I initially wanted to use random fact about cats however, all APIs with cat facts were either not functional or did not meet my requirements.

The app can provide a user, some more or less interesting information, which will be given by cats. The received information will be stored in a database and can be recalled. The application ensures, that the each useless fact, will always be "reminded" to the user by the same cat, who gave them the information in the first place.

## Building and running the application

The application requires Docker and .NET 10 (SDK should include the necessary runtimes) installed on the device.

To use the application first pull it from git or download it onto your device and make it a regular directory.

Open cmd/terminal/powershell in the app directory 
(`📁IntelligentConnector/`)

Type in command **docker-compose up -d** - this will pull a PostgreSQL database image to docker and create the necessary container. The database will be running on port 5432:5432 and will start automatically. The application has all the necessary login data hard-coded in `📁Data/DbContextFactory.cs`, since this project is not to be deployed anyway.

__NOTE:__ Keep in mind that since it handles database, containter is set to restart automatically. If it bothers you, change the docker-compose.yml

After the container is running, the application should be started with command **dotnet run** again in the app directory. It should run on http://localhost:5146 but just in case, you can check the address in the terminal output.

## Communicating via requests

The application provides 4 endpoint to communicate.

**[GET]**

* **/cat/newfact** - provides a new cat that gives you some random information
* **/cat/rememberfact** - a cat reminds you about the fact you already heard from it

Those two endpoints should be accessed via web browser for the best experience.  
Example of use: `http://localhost:5146/cat/rememberfact`

The application also provide endpoints for clearing the database:

**[DELETE]**

* **/cat/clearfacts** - empties the facts database
* **/cat/clearimages** - empties the database with seen cat images IDs

The application is designed to work even if those requests are never sent, however if you want for some reason to empty the database tables, these are helpful.

## Tests
The application contains a few automatic tests, that can be run using command **dotnet test** in app directory. To see what specific tests do, check `IntelligentConnector.ScenarioTests/CatEndpointScenarioTests.cs`

**NOTE:** Neither tests nor the app itself will work properly if you have no internet connection at the time.

---

# Public API Data Weaver - Random Facts From Cats (Wersja PL)

Program łączy funkcjonalności dwóch API z repozytorium [public-apis](https://github.com/public-apis/public-apis): 
* **Random Useless Facts** - https://uselessfacts.jsph.pl/ 
* **Cat as a Service (CATAAS)** - https://cataas.com/

Początkowo chciałem użyć losowych faktów o kotach, jednak wszystkie API z faktami o kotach albo nie działały, albo nie spełniały moich wymagań.

Aplikacja dostarcza użytkownikowi mniej lub bardziej interesujące informacje, które są przekazywane przez koty. Otrzymane informacje są przechowywane w bazie danych i mogą być przywołane ponownie. Aplikacja gwarantuje, że każdy bezużyteczny fakt będzie zawsze "przypominany" użytkownikowi przez tego samego kota, który przekazał mu tę informację za pierwszym razem.

## Budowanie i uruchamianie aplikacji

Aplikacja wymaga zainstalowanych na urządzeniu narzędzi Docker oraz .NET 10 (SDK powinno zawierać niezbędne środowiska uruchomieniowe).

Aby skorzystać z aplikacji, najpierw pobierz ją z gita (pull) lub pobierz na swoje urządzenie i umieść w zwykłym katalogu.

Otwórz cmd/terminal/powershell w katalogu aplikacji 
(`📁IntelligentConnector/`)

Wpisz komendę **docker-compose up -d** – spowoduje to pobranie obrazu bazy danych PostgreSQL do dockera i utworzenie niezbędnego kontenera. Baza danych będzie działać na porcie 5432:5432 i uruchomi się automatycznie. Aplikacja ma wszystkie niezbędne dane logowania wpisane na sztywno w pliku `📁Data/DbContextFactory.cs`, ponieważ projekt i tak nie jest przeznaczony do wdrożenia (deploymentu).

**UWAGA:** Ponieważ odpowiada za bazę danych, kontener jest ustawiony by włączać się automatycznie. Jeśli ci to przeszkadza, zmień ustawienia w docker-compose.yml

Po uruchomieniu kontenera aplikację należy uruchomić komendą **dotnet run**, ponownie w katalogu aplikacji. Powinna ona działać pod adresem http://localhost:5146, ale na wszelki wypadek możesz sprawdzić adres w danych wyjściowych terminala.

## Komunikacja poprzez żądania (requests)

Aplikacja udostępnia 4 punkty końcowe (endpoints) do komunikacji.

**[GET]**

* **/cat/newfact** – dostarcza nowego kota, który podaje losową informację.
* **/cat/rememberfact** – kot przypomina o fakcie, który już od niego usłyszałeś.

Dla najlepszych wrażeń te dwa punkty końcowe powinny być otwierane przez przeglądarkę internetową.  
Przykład użycia: `http://localhost:5146/cat/rememberfact`

Aplikacja udostępnia również punkty końcowe do czyszczenia bazy danych:

**[DELETE]**

* **/cat/clearfacts** – opróżnia bazę danych faktów.
* **/cat/clearimages** – opróżnia bazę danych z identyfikatorami widzianych obrazów kotów.

Aplikacja została zaprojektowana tak, aby działała nawet jeśli te żądania nigdy nie zostaną wysłane, jednak jeśli z jakiegoś powodu chcesz opróżnić tabele bazy danych, są one pomocne.

## Testy
Aplikacja zawiera kilka testów automatycznych, które można uruchomić za pomocą komendy **dotnet test** w katalogu aplikacji. Aby sprawdzić, co robią konkretne testy, zajrzyj do pliku `IntelligentConnector.ScenarioTests/CatEndpointScenarioTests.cs`

**UWAGA:** Ani testy, ani sama aplikacja nie będą działać poprawnie, jeśli w danym momencie nie masz połączenia z Internetem.