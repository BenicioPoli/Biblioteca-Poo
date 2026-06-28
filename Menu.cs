namespace SistemaBiblioteca {
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

		public static T ReadValid<T>(string prompt, Func<string, T?> parser, string failMessage) {
			while (true) {
				Console.Write($"{prompt}: ");

				string input = Console.ReadLine() ?? "";
				var t = parser(input);

				if (t == null) {
					Console.WriteLine(failMessage);
					continue;
				}

				return t!;
			}
		}

		public static string ReadNonEmpty(string prompt) {
			while (true) {
				Console.Write($"{prompt}: ");
				string input = Console.ReadLine();
				if (!string.IsNullOrEmpty(input)) return input;
				Console.WriteLine("El campo no puede estar vacío.");
			}
		}
	}
}

