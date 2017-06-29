using System;
using System.Collections.Generic;
using System.Text;

namespace AppMetrics.Common
{
	public static class CoreConstants
	{

        public enum Debug
        { 
            isDebug = 1
        }

		public enum Gender
		{
			Male = 1,
			Female = 2
		}

		public enum AlertType
		{
			Panic = 1
		}

    public enum AlertState
    {
      SinProcesar = 1,
      Procesada = 2
    }

		public enum MessageState
		{
			Unread = 1,
			Read = 2,
			Deleted = 3,
			Sent = 4
		}

		public enum UserStatus
		{
			Online = 1,
			Offline = 2
		}


		public enum UserType
		{
			AppTaxi = 1,
			WidgetUser = 2,
			WidgetAdmin = 3,
			CompanyAdmin = 4,
			AppClient = 5,
			Seller = 6,
            Telephonist = 7,
            CompanyClient = 8
		}

        public enum SaleUserType
        {
            Vendedor = 1,
            Auditor = 2,
            Administrador = 3,
        }

		public enum DaysOfWeek
		{
			Sunday = 1,
			Monday = 2,
			Tuesday = 3,
			Wednesday = 4,
			Thursday = 5,
			Friday = 6,
			Saturday = 7
		}

        public enum CarOwnerType
        {
            Owner = 1,
            Mandatary = 2
        }

		public enum PushEventType
		{
			Trips = 1,
			Messages = 2
		}

		public enum PushEventState
		{
			New = 1,
			Processed = 2
		}

		public enum PushEventUserState
		{
			Queued = 1,
			Sent = 2,
			SendError = 3,
			Received = 4
		}

		public enum TripsState
		{
			Pending = 1,
			Holding = 2,
			Inprogress = 3,
			Finished = 4,
			Cancel = 5,
			SystemFinished = 6,
			UnavailableTaxi = 7,
			ToExternal = 8,
            PendingAssigned = 9,
            Audit = 10
		}

		public enum TaxiState
		{
			Libre = 1,
			Ocupado = 2,
			EnCamino = 3,
			NoDisponible = 4,
			SinConexion = 5
		}

		public enum ExpiredAlerts
		{
			PolizaSeguro = 1,
			VerificacionTecnica = 2,
			Desinfeccion = 3,
			Licencia = 4,
			Licencia4 = 5
		}

		public enum MonitorAuditTypes
		{
			LogOut = 1,
			MonitorStateChanged = 2,
			TripReceived = 3,
			OpenedMonitor = 4,
			Login = 5
		}

        public enum TransactionState
        {
          Pendiente = 1,
          Aceptada = 2,
          Rechazada = 3
        }

        public enum PaymentMethod
        {
          PrePaid = 1,
          PostPaid = 2
        }

        public enum PaymentType
        { 
            Cash = 1,
            Account = 2,
            Code = 3
        }

        public enum BillingType
        { 
            NoFacturable = 1,
            PorEmpresa = 2,
            PorSector = 3,
            PorTarjeta = 4
        }

        public enum BillingPeriod
        {
            NoFacturable = 1,
            Semanal = 2,
            Quincenal = 3,
            Mensual = 4
        }

        public enum CashBoxState
        {
            Abierta = 1,
            Cerrada = 2,
            Auditada = 3,
            Rendida = 4,
            Completa = 5
        }

        public enum SaleTransactionState
        {
            Iniciada = 1,
            Informada = 2,
            Autorizada = 3,
            Completa = 4
        }

	}
}