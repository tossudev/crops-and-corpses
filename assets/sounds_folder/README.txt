	****************************************************
	*	CROPS & CORPSES GAME AUDIO README	   *
	*						   *
	*    sound design & music by Ilmari Apajalahti     *
	****************************************************


*	kaikkien tiedostojen formaatti on 24bit 48kHz stereo wav-tiedosto



*	kaikkea ääntä SAA käyttää kaupallisissa ja/tai ei-kaupallisissa yhteyksissä ja tarkoituksissa



*	"ambiences"-kansiosta löytyy päivä- ja yösyklin taustaäänet, näitä voi loopata crossfaden kanssa



*	"character sounds" löytyy kaikki pelaajahahmon tekemät äänet, tässä nyt vaan askeleet (steps) ja iskut (punches)

	askeleita löytyy 5 erilaista, näitä voi randomisti vaihdella samplesta toiseen kun liikkumista tapahtuu niin välttyy
	ns. konekivääriefektiltä, eli siltä että laukaistaan sama sample aina kun liikkuminen alkaa, varmaan joku 0.3 sek välein
	tapahtuva askel on hyvä lähtökohta rennolle kävelytahdille

	iskuja (punches) löytyy 3, näissä on sama homma, kannattaa randomisti vaihdella samplea aina kun lyödään



*	"zombies"-kansiossa on sekä yksittäisten zombien ääniä joita voi viljellä taustalle milloin halajaa:

	- "zombie breath"
	- "zombie growl" (kelasin että tää voi olla hyvä muuttumisääni zombieksi, jos kyläläiset voi muuttaa?)
	- "zombie hiss1"
	- "zombie hiss2"

	ja myös yleistä zombie-taustahälinää eli "zombie_crowd", tätä voi loopata crossfaden kanssa



*	"music"-kansiossa on musaa, omansa sekä päivälle, että yölle
	näistä on 2 eri versiota, "music_day" ja "music_night" ovat normiversiot jotka eivät looppaa "tahdissa"
	eli jos näitä haluaa käyttää niin musan kannattaa antaa soida loppuun asti ja sitten aloittaa alusta,
	tällöin tulee tietenkin pieni pätkä hiljaisuutta ennen kuin musa alkaa uudestaan

	jos haluaa että musa looppaa tauotta taustalla, tulee käyttää "loop"-versioita, nämä soivat saumattomasti
	peräjälkeen yhteen, ainoastaan päivämusasta vaihdettaessa yömusaan (ja päinvastoin) tulee musiikki feidata
	hitaasti pois/sisään

