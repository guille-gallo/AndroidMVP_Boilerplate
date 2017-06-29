//=============================================================================
// Sistema      : Clases de utilidades para paginas ASP.NET
// Archivo      : MailHelper.cs
// Autor        : E Beretta (e.beretta@assista.com)
// Actualizado  : 13/03/2012
// Compilador   : Microsoft Visual C#
//
// El archivo contiene una clase Helper para simplificar el envío de correos.
//
// Version  Fecha       Autor   Comentarios
// ============================================================================
// 1.0.0.0  21/10/2009  ENB     Creación de clase
// 1.0.0.1  13/03/2012  ENB     Creación método SendMailMessageWithFullErrors
//=============================================================================

using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Web.Configuration;
using System.Net.Configuration;
using System.IO;
using System.Web;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Collections;
using System.Reflection;
using AppMetrics.Logger;


namespace AppMetrics.Common {
	/// <summary>
	/// Proporciona operaciones comúnes para el envío de mails.
	/// </summary>
	public class MailHelper {


		#region Vars
	//	private MailSettingsSectionGroup _MailSettings;
		#endregion Vars
		private Log _logger;

		#region Constructor
		/// <summary>
		/// Constructor. 
		/// </summary>
		/// <remarks>
		/// Inicializa la sección mailSettings definida el archivo de configuración web.config.
		/// </remarks>
		public MailHelper(Log logger) {

			_logger = logger;
			//Configuration configurationFile = WebConfigurationManager.OpenWebConfiguration("~\\Web.config");
			//_MailSettings = (MailSettingsSectionGroup)configurationFile.GetSectionGroup("system.net/mailSettings");
		}
		#endregion Constructor

		#region SendMailAccountCreated
		/// <summary>
		/// Envía un mensaje con información de cuenta creada
		/// </summary>
		/// <param name="pFrom">Sender address</param>
		/// <param name="pTo">Recepient address</param>
		///	<param name="pBCC">Bcc recepient</param>
		/// <param name="pCC">Cc recepient</param>
		/// <param name="pSubject">Subject of mail message</param>
		/// <param name="pBody">Body of mail message</param>
		public void SendMailAccountCreated(string smtpHost, int smtpPort, string smtpUserName, string smtpPassword, string pFrom, string pTo, string pSubject, string pBody)
        {
            MailMessage oMailMessage = new MailMessage();

            try
            {
                oMailMessage.Subject = pSubject;
                oMailMessage.IsBodyHtml = true;
                oMailMessage.Priority = MailPriority.Normal;

                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(pBody, null, "text/html");

                LinkedResource logo_geo = new LinkedResource(HttpContext.Current.Server.MapPath(@"~\Images\logo_taxi.png"));
                logo_geo.ContentId = "logo_taxi";
                htmlView.LinkedResources.Add(logo_geo);

                LinkedResource head_escudo = new LinkedResource(HttpContext.Current.Server.MapPath(@"~\Images\head_escudo.png"));
                head_escudo.ContentId = "head_escudo";
                htmlView.LinkedResources.Add(head_escudo);

                LinkedResource ic_cuadriculado = new LinkedResource(HttpContext.Current.Server.MapPath(@"~\Images\ic_cuadriculado.png"));
                ic_cuadriculado.ContentId = "ic_cuadriculado";
                htmlView.LinkedResources.Add(ic_cuadriculado);

                LinkedResource ic_google_play = new LinkedResource(HttpContext.Current.Server.MapPath(@"~\Images\ic_google_play.png"));
                ic_google_play.ContentId = "ic_google_play";
                htmlView.LinkedResources.Add(ic_google_play);

                oMailMessage.AlternateViews.Add(htmlView);

                oMailMessage.From = new MailAddress(pFrom, "AppMetrics.net");
                oMailMessage.To.Add(new MailAddress(pTo));

            }
            catch (Exception ex)
            {
                _logger.TraceError("Error:" + ex.Message);
            }

            try
            {
							Utils.Utilities.SendMail(_logger, smtpHost, smtpPort, smtpUserName, smtpPassword, oMailMessage, false);
            }
            catch (Exception ex)
            {
                _logger.TraceError("Error:" + ex.Message);
            }
        }
		#endregion SendMailAccountCreated

		public void SendMailAlertExpired(string smtpHost, int smtpPort, string smtpUserName, string smtpPassword, string pFrom, string pTo, string pSubject, string pBody)
		{
			MailMessage oMailMessage = new MailMessage();

			try
			{
				oMailMessage.Subject = pSubject;
				oMailMessage.IsBodyHtml = true;
				oMailMessage.Priority = MailPriority.Normal;

				AlternateView htmlView = AlternateView.CreateAlternateViewFromString(pBody, null, "text/html");

                LinkedResource logo_geo = new LinkedResource(HttpContext.Current.Server.MapPath(@"~\Images\logo_taxi.png"));
				logo_geo.ContentId = "logo_taxi";
				htmlView.LinkedResources.Add(logo_geo);

				LinkedResource head_escudo = new LinkedResource(HttpContext.Current.Server.MapPath(@"~\Images\head_escudo.png"));
				head_escudo.ContentId = "head_escudo";
				htmlView.LinkedResources.Add(head_escudo);

				oMailMessage.AlternateViews.Add(htmlView);

				oMailMessage.From = new MailAddress(pFrom, "AppMetrics.net");
				oMailMessage.To.Add(new MailAddress(pTo));

			}
			catch (Exception ex)
			{
				_logger.TraceError("Error:" + ex.Message);
			}

			try
			{
				Utils.Utilities.SendMail(_logger, smtpHost, smtpPort, smtpUserName, smtpPassword, oMailMessage, false);
			}
			catch (Exception ex)
			{
				_logger.TraceError("Error:" + ex.Message);
			}
		}

    public void SendMailRequestTrips(string smtpHost, int smtpPort, string smtpUserName, string smtpPassword, string pFrom, string pTo, string pSubject, string pBody)
    {
      MailMessage oMailMessage = new MailMessage();

      try
      {
        oMailMessage.Subject = pSubject;
        oMailMessage.IsBodyHtml = true;
        oMailMessage.Priority = MailPriority.Normal;

        AlternateView htmlView = AlternateView.CreateAlternateViewFromString(pBody, null, "text/html");

        LinkedResource logo_geo = new LinkedResource(HttpContext.Current.Server.MapPath(@"~\Images\logo_taxi.png"));
        logo_geo.ContentId = "logo_taxi";
        htmlView.LinkedResources.Add(logo_geo);

        LinkedResource head_escudo = new LinkedResource(HttpContext.Current.Server.MapPath(@"~\Images\head_escudo.png"));
        head_escudo.ContentId = "head_escudo";
        htmlView.LinkedResources.Add(head_escudo);

        oMailMessage.AlternateViews.Add(htmlView);

        oMailMessage.From = new MailAddress(pFrom, "AppMetrics.net");
        oMailMessage.To.Add(new MailAddress(pTo));

      }
      catch (Exception ex)
      {
        _logger.TraceError("Error:" + ex.Message);
      }

      try
      {
        Utils.Utilities.SendMail(_logger, smtpHost, smtpPort, smtpUserName, smtpPassword, oMailMessage, false);
      }
      catch (Exception ex)
      {
        _logger.TraceError("Error:" + ex.Message);
      }
    }

    public void SendMailReplyRequestTrips(string smtpHost, int smtpPort, string smtpUserName, string smtpPassword, string pFrom, string pTo, string pSubject, string pBody)
    {
        MailMessage oMailMessage = new MailMessage();

        try
        {
            oMailMessage.Subject = pSubject;
            oMailMessage.IsBodyHtml = true;
            oMailMessage.Priority = MailPriority.Normal;

            AlternateView htmlView = AlternateView.CreateAlternateViewFromString(pBody, null, "text/html");

            LinkedResource logo_geo = new LinkedResource(HttpContext.Current.Server.MapPath(@"~\Images\logo_taxi.png"));
            logo_geo.ContentId = "logo_taxi";
            htmlView.LinkedResources.Add(logo_geo);

            LinkedResource head_escudo = new LinkedResource(HttpContext.Current.Server.MapPath(@"~\Images\head_escudo.png"));
            head_escudo.ContentId = "head_escudo";
            htmlView.LinkedResources.Add(head_escudo);

            oMailMessage.AlternateViews.Add(htmlView);

            oMailMessage.From = new MailAddress(pFrom, "AppMetrics.net");
            oMailMessage.To.Add(new MailAddress(pTo));

        }
        catch (Exception ex)
        {
            _logger.TraceError("Error:" + ex.Message);
        }

        try
        {
            Utils.Utilities.SendMail(_logger, smtpHost, smtpPort, smtpUserName, smtpPassword, oMailMessage, false);
        }
        catch (Exception ex)
        {
            _logger.TraceError("Error:" + ex.Message);
        }
    }

    public void SendMailFewTrips(string smtpHost, int smtpPort, string smtpUserName, string smtpPassword, string pFrom, string pTo, string pSubject, string pBody)
    {
        MailMessage oMailMessage = new MailMessage();

        try
        {
            oMailMessage.Subject = pSubject;
            oMailMessage.IsBodyHtml = true;
            oMailMessage.Priority = MailPriority.Normal;

            AlternateView htmlView = AlternateView.CreateAlternateViewFromString(pBody, null, "text/html");

            LinkedResource logo_geo = new LinkedResource(HttpContext.Current.Server.MapPath(@"~\Images\logo_taxi.png"));
            logo_geo.ContentId = "logo_taxi";
            htmlView.LinkedResources.Add(logo_geo);

            LinkedResource head_escudo = new LinkedResource(HttpContext.Current.Server.MapPath(@"~\Images\head_escudo.png"));
            head_escudo.ContentId = "head_escudo";
            htmlView.LinkedResources.Add(head_escudo);

            oMailMessage.AlternateViews.Add(htmlView);

            oMailMessage.From = new MailAddress(pFrom, "AppMetrics.net");
            oMailMessage.To.Add(new MailAddress(pTo));

        }
        catch (Exception ex)
        {
            _logger.TraceError("Error:" + ex.Message);
        }

        try
        {
            Utils.Utilities.SendMail(_logger, smtpHost, smtpPort, smtpUserName, smtpPassword, oMailMessage, false);
        }
        catch (Exception ex)
        {
            _logger.TraceError("Error:" + ex.Message);
        }
    }

    public void SendMailFewTripsToSeller(string smtpHost, int smtpPort, string smtpUserName, string smtpPassword, string pFrom, string pTo, string pSubject, string pBody)
    {
      MailMessage oMailMessage = new MailMessage();

      try
      {
        oMailMessage.Subject = pSubject;
        oMailMessage.IsBodyHtml = true;
        oMailMessage.Priority = MailPriority.Normal;

        AlternateView htmlView = AlternateView.CreateAlternateViewFromString(pBody, null, "text/html");

        LinkedResource logo_geo = new LinkedResource(HttpContext.Current.Server.MapPath(@"~\Images\logo_taxi.png"));
        logo_geo.ContentId = "logo_taxi";
        htmlView.LinkedResources.Add(logo_geo);

        LinkedResource head_escudo = new LinkedResource(HttpContext.Current.Server.MapPath(@"~\Images\head_escudo.png"));
        head_escudo.ContentId = "head_escudo";
        htmlView.LinkedResources.Add(head_escudo);

        oMailMessage.AlternateViews.Add(htmlView);

        oMailMessage.From = new MailAddress(pFrom, "AppMetrics.net");
        oMailMessage.To.Add(new MailAddress(pTo));

      }
      catch (Exception ex)
      {
        _logger.TraceError("Error:" + ex.Message);
      }

      try
      {
        Utils.Utilities.SendMail(_logger, smtpHost, smtpPort, smtpUserName, smtpPassword, oMailMessage, false);
      }
      catch (Exception ex)
      {
        _logger.TraceError("Error:" + ex.Message);
      }
    }

		#region MailUserWithOutLogin
		/// <summary>
		/// Envía un mensaje para los usuarios que no la usaron
		/// </summary>
		/// <param name="pFrom">Sender address</param>
		/// <param name="pTo">Recepient address</param>
		///	<param name="pBCC">Bcc recepient</param>
		/// <param name="pCC">Cc recepient</param>
		/// <param name="pSubject">Subject of mail message</param>
		/// <param name="pBody">Body of mail message</param>
		public void MailUserWithOutLogin(string smtpHost, int smtpPort, string smtpUserName, string smtpPassword, string pFrom, string pTo, string pSubject, string pBody)
		{
			MailMessage oMailMessage = new MailMessage();

			try
			{
				oMailMessage.Subject = pSubject;
				oMailMessage.IsBodyHtml = true;
				oMailMessage.Priority = MailPriority.Normal;

				AlternateView htmlView = AlternateView.CreateAlternateViewFromString(pBody, null, "text/html");

				LinkedResource logo_geo = new LinkedResource(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"\Images\logo_taxi.png");
				logo_geo.ContentId = "logo_taxi";
				htmlView.LinkedResources.Add(logo_geo);

				LinkedResource mokp_01 = new LinkedResource(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"\Images\mokp_01.png");
				mokp_01.ContentId = "mokp_01";
				htmlView.LinkedResources.Add(mokp_01);

				LinkedResource mokp_02 = new LinkedResource(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"\Images\mokp_02.png");
				mokp_02.ContentId = "mokp_02";
				htmlView.LinkedResources.Add(mokp_02);

				LinkedResource mokp_03 = new LinkedResource(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"\Images\mokp_03.png");
				mokp_03.ContentId = "mokp_03";
				htmlView.LinkedResources.Add(mokp_03);

				LinkedResource ic_cuadriculado = new LinkedResource(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"\Images\ic_cuadriculado.png");
				ic_cuadriculado.ContentId = "ic_cuadriculado";
				htmlView.LinkedResources.Add(ic_cuadriculado);

				LinkedResource ic_google_play = new LinkedResource(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"\Images\ic_google_play.png");
				ic_google_play.ContentId = "ic_google_play";
				htmlView.LinkedResources.Add(ic_google_play);

				oMailMessage.AlternateViews.Add(htmlView);

				oMailMessage.From = new MailAddress(pFrom, "AppMetrics.net");
				oMailMessage.To.Add(new MailAddress(pTo));

			}
			catch (Exception ex)
			{
				_logger.TraceError("Error:" + ex.Message);
			}

			try
			{
				Utils.Utilities.SendMail(_logger, smtpHost, smtpPort, smtpUserName, smtpPassword, oMailMessage, false);
			}
			catch (Exception ex)
			{
				_logger.TraceError("Error:" + ex.Message);
			}
		}
		#endregion MailUserWithOutLogin

		#region MailToUserRequest
		/// <summary>
		/// Envía un mensaje para los usuarios que no la usaron
		/// </summary>
		/// <param name="pFrom">Sender address</param>
		/// <param name="pTo">Recepient address</param>
		///	<param name="pBCC">Bcc recepient</param>
		/// <param name="pCC">Cc recepient</param>
		/// <param name="pSubject">Subject of mail message</param>
		/// <param name="pBody">Body of mail message</param>
		public void MailToUserRequest(string smtpHost, int smtpPort, string smtpUserName, string smtpPassword, string pFrom, string pTo, string pSubject, string pBody)
		{
			MailMessage oMailMessage = new MailMessage();

			try
			{
				oMailMessage.Subject = pSubject;
				oMailMessage.IsBodyHtml = true;
				oMailMessage.Priority = MailPriority.Normal;

				AlternateView htmlView = AlternateView.CreateAlternateViewFromString(pBody, null, "text/html");

				LinkedResource logo_geo = new LinkedResource(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"\Images\logo_taxi.png");
				logo_geo.ContentId = "logo_taxi";
				htmlView.LinkedResources.Add(logo_geo);

				LinkedResource email_ico_01 = new LinkedResource(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"\Images\email_ico_01.png");
				email_ico_01.ContentId = "email_ico_01";
				htmlView.LinkedResources.Add(email_ico_01);

				LinkedResource email_ico_02 = new LinkedResource(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"\Images\email_ico_02.png");
				email_ico_02.ContentId = "email_ico_02";
				htmlView.LinkedResources.Add(email_ico_02);

				LinkedResource email_ico_03 = new LinkedResource(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"\Images\email_ico_03.png");
				email_ico_03.ContentId = "email_ico_03";
				htmlView.LinkedResources.Add(email_ico_03);

				LinkedResource ic_cuadriculado = new LinkedResource(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"\Images\ic_cuadriculado.png");
				ic_cuadriculado.ContentId = "ic_cuadriculado";
				htmlView.LinkedResources.Add(ic_cuadriculado);

				LinkedResource ic_google_play = new LinkedResource(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"\Images\ic_google_play.png");
				ic_google_play.ContentId = "ic_google_play";
				htmlView.LinkedResources.Add(ic_google_play);

				oMailMessage.AlternateViews.Add(htmlView);

				oMailMessage.From = new MailAddress(pFrom, "AppMetrics.net");
				oMailMessage.To.Add(new MailAddress(pTo));

			}
			catch (Exception ex)
			{
				_logger.TraceError("Error:" + ex.Message);
			}

			try
			{
				Utils.Utilities.SendMail(_logger, smtpHost, smtpPort, smtpUserName, smtpPassword, oMailMessage, false);
			}
			catch (Exception ex)
			{
				_logger.TraceError("Error:" + ex.Message);
			}
		}
		#endregion MailToUserRequest

	}

}
