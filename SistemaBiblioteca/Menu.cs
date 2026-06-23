namespace ClinicaMedica {
	public static class Menu {
		public static T Choose<T>(string prompt, List<T> items, Func<T, string> displaySelector) {
			if (items == null || items.Count == 0)
				throw new ArgumentException("The item list cannot be empty.");

			while (true) {
				Console.WriteLine($"{prompt}");
				for (int i = 0; i < items.Count; i++) {
					Console.WriteLine($"[{i + 1}] {displaySelector(items[i])}");
				}

				Console.Write("Selecciona una opción: ");
				if (int.TryParse(Console.ReadLine(), out int choice) && choice >= 1 && choice <= items.Count) {
					return items[choice - 1];
				}

				Console.WriteLine($"Fuera de rango.  Ingresa un valor entre 1 y {items.Count}.");
			}
		}

		public static void RunCommand(string title, List<(string Description, Action Callback)> commands) {
			while (true) {
				Console.WriteLine($"{title}");
				for (int i = 0; i < commands.Count; i++) {
					Console.WriteLine($"[{i + 1}] {commands[i].Description}");
				}

				Console.WriteLine("[0] Salir");

				Console.Write("Selecciona una opción: ");
				if (int.TryParse(Console.ReadLine(), out int choice)) {
					if (choice == 0) break;
					if (choice >= 1 && choice <= commands.Count) {
						commands[choice - 1].Callback();
						continue;
					}
				}

				Console.WriteLine("Opción no valida, probá de nuevo.");
			}
		}

		public static bool ConfirmPositive(string prompt) {
			while (true) {
				Console.Write($"{prompt} [S/n]: ");
				string input = Console.ReadLine()?.Trim().ToLower() ?? "s";
				if (input == "s" || input == "si" || input == "y" || input == "yes") return true;
				if (input == "n" || input == "no") return false;
				Console.WriteLine("Por favor ingrese [Si] o [no].");
			}
		}

		public static bool ConfirmNegative(string prompt) {
			while (true) {
				Console.Write($"{prompt} [s/N]: ");
				string input = Console.ReadLine()?.Trim().ToLower() ?? "n";
				if (input == "s" || input == "si" || input == "y" || input == "yes") return true;
				if (input == "n" || input == "no") return false;
				Console.WriteLine("Por favor ingrese [si] o [No].");
			}
		}

		public static string ReadFutureDate(string prompt, List<Disponibilidad> activeSchedules) {
			// hdps en C# cómo que si no arranca el día en domingo anda mal
			string[] dias = {"Domingo", "Lunes", "Martes", "Miércoles", "Jueves", "Viernes", "Sábado"};

			while (true) {
				Console.Write($"{prompt} (AAAA-MM-DD): ");
				string input = Console.ReadLine() ?? "";

				if (DateTime.TryParse(input, out DateTime parsedDate) && parsedDate >= DateTime.Today) {
					int dayIndex = (int)parsedDate.DayOfWeek;

					if (activeSchedules.Any(d => d.DiaSemana == dayIndex)) {
						return input;
					}

					Console.WriteLine($"No hay turnos un día {dias[dayIndex]}.  Elija otra fecha.");
				} else {
					Console.WriteLine("Fecha inválida.  Debe ser hoy o una fecha futura (AAAA-MM-DD.)");
				}
			}
		}

		public static string ReadValidTime(string prompt, string chosenDate, List<Disponibilidad> activeSchedules) {
			DateTime parsedDate = DateTime.Parse(chosenDate);
			int dayIndex = (int)parsedDate.DayOfWeek;

			while (true) {
				Console.Write($"{prompt} (HH:MM): ");
				string input = Console.ReadLine() ?? "";

				if (TimeSpan.TryParse(input, out TimeSpan inputTime)) {
					bool match = activeSchedules.Any(d => 
							d.DiaSemana == dayIndex && 
							TimeSpan.Parse(d.HoraInicio) <= inputTime && 
							TimeSpan.Parse(d.HoraFin) >= inputTime);

					if (match) return input;
					Console.WriteLine("La hora ingresada está fuera del rango de atención para este día.");
				} else {
					Console.WriteLine("Formato de hora inválido.  Usá HH:MM.");
				}
			}
		}
	}
}
