-- A partir de nombres de socios y libros se generan prestamos y reservas
-- según la cantidad deseada.  Este script está hecho para darle datos de
-- testing a la base de datos por primera vez, pero no corresponden a datos
-- reales de ningún modo.
--
-- Este script está diseñado para correrse de la siguiente forma:
-- > lua popularBD.lua | sqlite3 Biblioteca.db

local people = {
	{nombre = "Casey",	apellido = "Muratori"};
	{nombre = "Alexey",	apellido = "Kutepov"};
	{nombre = "Andrew",	apellido = "Kelley"};
	{nombre = "John",	apellido = "Price"};
	{nombre = "Soap",	apellido = "MacTavish"};
	{nombre = "Kyle",	apellido = "Garrick"};
	{nombre = "Simon",	apellido = "Riley"};
	{nombre = "Alex",	apellido = "Mason"};
	{nombre = "David",	apellido = "Mason"};
	{nombre = "Frank",	apellido = "Woods"};
	{nombre = "Jason",	apellido = "Hudson"};
	{nombre = "Raúl",	apellido = "Menendez"};
	{nombre = "Albert",	apellido = "Arlington"};
	{nombre = "Billy",	apellido = "Handsome"};
	{nombre = "Finn",	apellido = "O'Leary"};
	{nombre = "Salvatore",	apellido = "DeLuca"};
	{nombre = "Edward",	apellido = "Richtofen"};
	{nombre = "Ludwig",	apellido = "Maxis"};
	{nombre = "Samantha",	apellido = "Maxis"};
	{nombre = "Paul",	apellido = "Leskowitz"};
	{nombre = "Lina",	apellido = "Leskowitz"};
	{nombre = "Tiara",	apellido = "Leskowitz"};
	{nombre = "Anna",	apellido = "Mark"};
	{nombre = "Marvin",	apellido = "Mark"};
	{nombre = "Carrie",	apellido = "Mark"};
	{nombre = "Michael",	apellido = "Hammond"};
	{nombre = "Rainer",	apellido = "Newmaker"};
	{nombre = "Will",	apellido = "Toledo"};
	{nombre = "Ethan",	apellido = "Ives"};
	{nombre = "Andew",	apellido = "Katz"};
	{nombre = "Seth",	apellido = "Dalby"};
};

local books = {
	{ titulo = "Confessions of the Fox",	autor = "Jordy Rosemberg"
	, genres = {"Ficción histórica especulativa", "bastante inapropiado"}};

	{ titulo = "A Tip for the Hangman",	autor = "Allison Epstein"
	, genres = {"Ficción histórica especulativa", "Misterio", "Romance", "LGBT"}};

	{ titulo = "This Is How You Lose The Time War",	autor = "Amal El-Mohtar"
	, genres = {"Ciencia Ficción", "Ficción", "Romance"}};

	{ titulo = "The Song of Achilles",	autor = "Madeline Miller"
	, genres = {"Fantasía", "Ficción histórica", "Mitología", "Romance", "LGBT"}};

	{ titulo = "Circe",	autor = "Madeline Miller"
	, genres = {"Fantasía", "Ficción histórica", "Mitología", "Mitología griega"}};

	{ titulo = "Let the Dead Bury the Dead",	autor = "Allison Epstein"
	, genres = {"Ficción histórica especulativa", "Romance", "LGBT"}};

	{ titulo = "Tarnished Are the Stars",	autor = "Rosiee Thor"
	, genres = {"Ciencia ficción", "Ficción", "Steampunk", "LGBT"}};

	{ titulo = "The Bone Witch",	autor = "Rin Chupeco"
	, genres = {"Fantasía", "Magia", "Ficción", "Paranormal", "Supernatural"}};

	{ titulo = "The Heart Forger",	autor = "Rin Chupeco"
	, genres = {"Fantasía", "Magia", "Ficción", "Paranormal", "Romance"}};

	{ titulo = "The Shadowglass",	autor = "Rin Chupeco"
	, genres = {"Fantasía", "Magia", "Ficción", "Paranormal"}};

	{ titulo = "The City & The City",	autor = "China Miéville"
	, genres = {"Fantasía", "Ficción", "Ciencia Ficción", "Crimen", "Misterio"}};

	{ titulo = "How to Stop Missing my Twice Dead Wife",	autor = "???"
	, genres = {"Autoayuda"}};

	{ titulo = "Heart of Iron",	autor = "C. M. Alongi"
	, genres = {"Fantasía", "LGBT", "Magia", "Fae", "Urbano"}};

	{ titulo = "El Nacimiento de Rabasedas",	autor = "Eva Atenas"
	, genres = {"Masculino", "Autobiografía"}};

	{ titulo = "El Libro Troll",	autor = "El Rubius"
	, genres = {"Masculino", "Humor"}};

	{ titulo = "Cómo conocí a tus Grafos",	autor = "Regina Muzzulini"
	, genres = {"Biografía", "Ciencia", "Telenovela"}};

	{ titulo = "How I made $290,000 selling books",	autor = "Jack Stratton"
	, genres = {"Autoayuda", "Economía"}};
};

local totalPrestamos = 50;
local totalReservas = 75;

math.randomseed(999);

local tipoSocio = {"Comun", "Estudiante", "Docente"};
local tipoSocioAssoc = {["Comun"] = 1, ["Estudiante"] = 2, ["Docente"] = 3};

print("BEGIN TRANSACTION;");

local genres = {};
for _, book in ipairs(books) do
	book.genres = book.genres or {"General"};
	for _, name in ipairs(book.genres) do
		genres[name] = true;
	end
end

local genresID = {};
local genreCount = 1;
for name, _ in pairs(genres) do
	genresID[name] = tostring(genreCount);
	print(string.format("INSERT INTO \"Genero\" (\"ID\", \"Descripcion\") VALUES ('%s', '%s');", tostring(genreCount), name));
	genreCount = genreCount + 1;
end

for i, person in ipairs(people) do
	person.nro_socio = i;
	if not person.tipo then
		person.tipo = tipoSocio[((i - 1) % #tipoSocio) + 1];
	end

	if not person.email then
		local providers = {"gmail.com", "yahoo.com.ar", "outlook.com", "ips.edu.ar"};
		person.email = string.format("%s%s%s%s@%s", person.nombre:gsub("%W", ""), math.random(0, 99), person.apellido:gsub("%W", ""), math.random(0, 99), providers[math.random(1, #providers)]);
	end

	local idTipo = tipoSocioAssoc[person.tipo] or 1;
	print(string.format("INSERT INTO \"Socio\" (\"NroSocio\", \"Nombre\", \"Apellido\", \"Email\", \"TipoSocio\", \"Activo\") VALUES (%d, '%s', '%s', '%s', %d, 1);", person.nro_socio, person.nombre:gsub("'", "\'"), person.apellido:gsub("'", "''"), person.email, idTipo));
end

local bookSet = {}
for _, book in ipairs(books) do
	if not book.isbn then
		book.isbn = string.format("978316148%04d", math.random(1000, 9999));
	end;

	if not book.copias then
		book.copias = math.random(3, 22);
	end;

	bookSet[book.isbn] = book.copias;
	print(string.format("INSERT INTO \"Libro\" (\"ISBN\", \"Titulo\", \"Autor\", \"CantidadCopias\") VALUES ('%s', '%s', '%s', %d);", book.isbn, book.titulo:gsub("'", "''"), book.autor:gsub("'", "''"), book.copias));

	for _, genre in ipairs(book.genres) do
		print(string.format("INSERT INTO \"LibroGenero\" (\"ISBN\", \"GeneroId\") VALUES ('%s', '%s');", book.isbn, genresID[genre]))
	end
end

local loanTracks = {}
local loanCount = 0
local loopCount = 0

local status = 0;
while loanCount < totalPrestamos and loopCount < 5000 do
	loopCount = loopCount + 1;
	local person = people[math.random(#people)];
	local book = books[math.random(#books)];
	local key = person.nro_socio .. "_" .. book.isbn;

	if not loanTracks[key] and bookSet[book.isbn] > 0 then
		loanTracks[key] = true;
		bookSet[book.isbn] = bookSet[book.isbn] - 1;

		status = (status % 3) + 1;
		local date_start = 1774281600 - (math.random(5, 20) * 86400);
		local date_end = date_start + (14 * 86400);

		print(string.format("INSERT INTO \"Prestamo\" (\"Socio\", \"Libro\", \"FechaPrestamo\", \"FechaVencimiento\", \"FechaDevolucion\", \"Estado\") VALUES (%d, '%s', %d, %d, NULL, %d);", person.nro_socio, book.isbn, date_start, date_end, status));

		loanCount = loanCount + 1;
	end
end

local reserveTracks = {};
local reserveCount = 0;
loopCount = 0;
status = 0;

while reserveCount < totalReservas and loopCount < 5000 do
	loopCount = loopCount + 1;

	local person = people[math.random(#people)];
	local book = books[math.random(#books)];
	local tracking_key = person.nro_socio .. "_" .. book.isbn;

	if not loanTracks[tracking_key] and not reserveTracks[tracking_key] then
		reserveTracks[tracking_key] = true;

		status = (status % 3) + 1;
		local date_reserve = string.format("2026-06-%02d", math.random(1, 21));

		print(string.format("INSERT INTO \"Reserva\" (\"Socio\", \"Libro\", \"FechaReserva\", \"Estado\") VALUES (%d, '%s', '%s', %d);", person.nro_socio, book.isbn, date_reserve, status));

		reserveCount = reserveCount + 1;
	end
end

print("COMMIT;");
