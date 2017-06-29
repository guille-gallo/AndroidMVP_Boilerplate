using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Data;
using System.Reflection;
using System.Globalization;
using System.Configuration;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Security.Cryptography;

using zlib;
using AppMetrics.Logic;
using AppMetrics.Common;
using AppMetrics.Common.Utils;
using AppMetrics.Common.Security;
using AppMetrics.Data.ADONETDataAccess;
using Newtonsoft.Json;

namespace AppMetrics.Logic.Utils
{
	/// <summary>
	/// Utilidades
	/// </summary>
	public static class CoreUtils {

        /// <summary>
        /// Ejecuta el QueryString pasado como parametro y devuelve
        /// un DataTable o DataReader segun el tipo de oResult ingresado.
        /// </summary>
        /// <param name="oDA">Objeto DataAccess sobre el cual se ejecuta la query</param>
        /// <param name="strSQL">QueryString a ejecutar</param>
        /// <param name="oResult">Tipo de datos a devolver (puede ser un DataTable o un SqlDataReader</param>
        public static void saveCallData(List<Model.CallData> data, string path, string FileName)
        {
            try
            {
                string output = JsonConvert.SerializeObject(data);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                //File.WriteAllText(Directory.GetCurrentDirectory()+"\\"+path + "\\" + FileName, output);
                File.WriteAllText(path + "\\" + FileName, output);
            }
            catch (Exception ex)
            {
                throw new CoreException("Error no esperado al guardar el archivo: "+ex.Message + "\r\nPath: "+path+"\r\nFileName: "+FileName, ex);
            }
        }

        /// <summary>
        /// Ejecuta el QueryString pasado como parametro y devuelve
        /// un DataTable o DataReader segun el tipo de oResult ingresado.
        /// </summary>
        /// <param name="oDA">Objeto DataAccess sobre el cual se ejecuta la query</param>
        /// <param name="strSQL">QueryString a ejecutar</param>
        /// <param name="oResult">Tipo de datos a devolver (puede ser un DataTable o un SqlDataReader</param>
        public static List<Model.CallData> restoreCallData(string path, string FileName)
        {
            try
            {
                string data = File.ReadAllText(path + "\\" + FileName);
                List<Model.CallData> output = JsonConvert.DeserializeObject<List<Model.CallData>>(data);
                return output;
            }
            catch (Exception ex)
            {
                throw new CoreException("Error no esperado al guardar el archivo", ex);
            }
        }

		/// <summary>
		/// Ejecuta el QueryString pasado como parametro y devuelve
		/// un DataTable o DataReader segun el tipo de oResult ingresado.
		/// </summary>
		/// <param name="oDA">Objeto DataAccess sobre el cual se ejecuta la query</param>
		/// <param name="strSQL">QueryString a ejecutar</param>
		/// <param name="oResult">Tipo de datos a devolver (puede ser un DataTable o un SqlDataReader</param>
		public static void ExcecuteSQL(DataAccess oDA, string strSQL, ref object oResult) {
			DataAccess oDataAccess = oDA;

			try {
				if (oResult == null) {
					throw new CoreException("El objeto 'oResult' no debe ser nulo.", 1);
				}

				if (oResult is DataTable) {
					oResult = oDataAccess.GetDataTable(strSQL);
				} else if (oResult is object) {
					oResult = oDataAccess.ExecuteReader(strSQL);
				} else {
					throw new CoreException("El objeto 'oResult' es incorrecto. Verifique que sea un DataTable o un Object.", 2);
				}
			} catch (DataAccessException ex) {
				throw new CoreException(ex.Message, ex);
			} catch (CoreException ex) {
				throw ex;
			} catch (Exception ex) {
				throw new CoreException("Error no esperado al ejecutar la consulta", ex);
			}
		}

		/// <summary>
		/// Obtiene el nombre de la base de datos actual.
		/// </summary>
		/// <param name="oDA">Conexión actual.</param>
		/// <returns>Nombre de la Base de Datos.</returns>
		public static string GetBaseName(DataAccess oDA) {
			try {
				DataTable dt = null;
				oDA.ClearParameters();
				dt = oDA.GetDataTable("SELECT DB_NAME()");
				return dt.Rows[0][0].ToString();
			} catch (DataAccessException ex) {
				throw new CoreException(ex.Message, ex);
			} catch (Exception ex) {
				throw new CoreException("Error no esperado al obtener el nombre de la Base.", ex);
			}
		}

		/// <summary>
		/// Genera una clave aleatoria. NOTA: Se puede utilizar dentro de bucles, pero se le tiene que pasar un objeto Random que este generado fuera del bucle.
		/// </summary>
		/// <param name="numeric">Indica si la clave debe ser solo numérica.</param>
		/// <param name="oRandom">Objeto Random</param>
		/// <returns>string con la clave.</returns>
		public static string GeneraClave(bool numeric, Random oRandom) {
			string letras = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
			string caracter = "";
			string password = "";
			int contNumeros = 0;
			int contLetras = 0;
			int random = 0;
			int start = 0;
			int carAscci = 0;
			int oldAscci = 0;
			double junk = 0;

			try {
				if (numeric) {
					start = 27;
				} else {
					start = 1;
				}

				random = oRandom.Next(start, letras.Length);

				password = letras.Substring(random, 1);
				oldAscci = Convert.ToInt32(Convert.ToChar(password));

				//Si es numerico
				if (double.TryParse(password, System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out junk)) {
					contNumeros += 1;
				} else {
					contLetras += 1;
				}

				if (numeric) {
					for (int i = 2; i <= 6; i++) {
						do {
							random = oRandom.Next(start, letras.Length);
							caracter = letras.Substring(random, 1);

							carAscci = Convert.ToInt32(Convert.ToChar(caracter));
						} while (carAscci == oldAscci ||
										 carAscci == oldAscci + 1 ||
										 carAscci == oldAscci - 1);

						password += caracter;
						oldAscci = carAscci;
					}
				} else {
					for (int i = 2; i <= 6; i++) {
						if (contNumeros != 3 && contLetras != 3) {
							do {
								random = oRandom.Next(1, letras.Length);
								caracter = letras.Substring(random, 1);

								carAscci = Convert.ToInt32(Convert.ToChar(caracter));
							} while (carAscci == oldAscci ||
											 carAscci == oldAscci + 1 ||
											 carAscci == oldAscci - 1);

						} else if (contNumeros == 3) {
							do {
								random = oRandom.Next(1, letras.Length);
								caracter = letras.Substring(random, 1);

								carAscci = Convert.ToInt32(Convert.ToChar(caracter));
							} while (carAscci == oldAscci ||
											 carAscci == oldAscci + 1 ||
											 carAscci == oldAscci - 1 ||
											 double.TryParse(caracter, System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out junk));
						} else {
							do {
								random = oRandom.Next(1, letras.Length);
								caracter = letras.Substring(random, 1);

								carAscci = Convert.ToInt32(Convert.ToChar(caracter));
							} while (carAscci == oldAscci ||
										 carAscci == oldAscci + 1 ||
										 carAscci == oldAscci - 1 ||
										 !double.TryParse(caracter, System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out junk));
						}

						password += caracter;
						oldAscci = carAscci;

						//Si es numerico
						if (double.TryParse(caracter, System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out junk)) {
							contNumeros += 1;
						} else {
							contLetras += 1;
						}
					}
				}

				return password;
			} catch (Exception) {
				return "";
			} finally {
				oRandom = null;
			}
		}

		/// <summary>
		/// Genera una clave aleatoria. NOTA: no se puede utilizar dentro de bucles.
		/// </summary>
		/// <param name="numeric">Indica si la clave debe ser solo numérica.</param>
		/// <returns>string con la clave.</returns>
		public static string GeneraClave(bool numeric) {
			string letras = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
			string caracter = "";
			string password = "";
			int contNumeros = 0;
			int contLetras = 0;
			int random = 0;
			int start = 0;
			int carAscci = 0;
			int oldAscci = 0;
			double junk = 0;
			int seed = 0;
			Random oRandom = null;

			try {
				seed = (int)DateTime.Now.Ticks;
				oRandom = new Random(seed);

				if (numeric) {
					start = 27;
				} else {
					start = 1;
				}

				random = oRandom.Next(start, letras.Length);

				password = letras.Substring(random, 1);
				oldAscci = Convert.ToInt32(Convert.ToChar(password));

				//Si es numerico
				if (double.TryParse(password, System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out junk)) {
					contNumeros += 1;
				} else {
					contLetras += 1;
				}

				if (numeric) {
					for (int i = 2; i <= 6; i++) {
						do {
							random = oRandom.Next(start, letras.Length);
							caracter = letras.Substring(random, 1);

							carAscci = Convert.ToInt32(Convert.ToChar(caracter));
						} while (carAscci == oldAscci ||
										 carAscci == oldAscci + 1 ||
										 carAscci == oldAscci - 1);

						password += caracter;
						oldAscci = carAscci;
					}
				} else {
					for (int i = 2; i <= 6; i++) {
						if (contNumeros != 3 && contLetras != 3) {
							do {
								random = oRandom.Next(1, letras.Length);
								caracter = letras.Substring(random, 1);

								carAscci = Convert.ToInt32(Convert.ToChar(caracter));
							} while (carAscci == oldAscci ||
											 carAscci == oldAscci + 1 ||
											 carAscci == oldAscci - 1);

						} else if (contNumeros == 3) {
							do {
								random = oRandom.Next(1, letras.Length);
								caracter = letras.Substring(random, 1);

								carAscci = Convert.ToInt32(Convert.ToChar(caracter));
							} while (carAscci == oldAscci ||
											 carAscci == oldAscci + 1 ||
											 carAscci == oldAscci - 1 ||
											 double.TryParse(caracter, System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out junk));
						} else {
							do {
								random = oRandom.Next(1, letras.Length);
								caracter = letras.Substring(random, 1);

								carAscci = Convert.ToInt32(Convert.ToChar(caracter));
							} while (carAscci == oldAscci ||
										 carAscci == oldAscci + 1 ||
										 carAscci == oldAscci - 1 ||
										 !double.TryParse(caracter, System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out junk));
						}

						password += caracter;
						oldAscci = carAscci;

						//Si es numerico
						if (double.TryParse(caracter, System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out junk)) {
							contNumeros += 1;
						} else {
							contLetras += 1;
						}
					}
				}

				return password;
			} catch (Exception) {
				return "";
			} finally {
				oRandom = null;
			}
		}

		/// <summary>
		/// CalculateCRC32
		/// </summary>
		/// <param name="strData"></param>
		/// <returns></returns>
		public static string CalculateCRC32(string strData) {
			clsCrc32 objCrc32;

			try {
				objCrc32 = new clsCrc32();
				byte[] rawBytes = System.Text.ASCIIEncoding.ASCII.GetBytes(strData);
				objCrc32.Update(rawBytes);
				return objCrc32.Value.ToString();
			} catch (Exception ex) {
				throw ex;
			} finally {
				objCrc32 = null;
			}
		}

		/// <summary>
		/// GetToken
		/// </summary>
		/// <param name="length"></param>
		/// <returns></returns>
		public static string GetToken(int length) {
			RNGCryptoServiceProvider csprng = new RNGCryptoServiceProvider();
			byte[] token = new byte[length];
			csprng.GetBytes(token);
			return Convert.ToBase64String(token);
		}

		internal static bool ValidateUrl(string url) {
			if (url == null || url == "") return false;
			System.Text.RegularExpressions.Regex oRegExp = new System.Text.RegularExpressions.Regex(@"(http|ftp|https)://([\w-]+\.)+(/[\w- ./?%&=]*)?", System.Text.RegularExpressions.RegexOptions.IgnoreCase); 
			return oRegExp.Match(url).Success;
		}

	}
}