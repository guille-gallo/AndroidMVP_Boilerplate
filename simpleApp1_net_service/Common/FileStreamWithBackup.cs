using System;
using System.IO;

namespace TaxiGuardian.Logger
{
  public sealed class FileStreamWithBackup : FileStream
  {
	  #region Constructors
	  public FileStreamWithBackup(string path, long maxFileLength, FileMode mode)
	    : base(path, BaseFileMode(mode), FileAccess.Write)
	  {
	    Init(path, maxFileLength, mode);
	  }

	  public FileStreamWithBackup(string path, long maxFileLength, FileMode mode, FileShare share)
      : base(path, BaseFileMode(mode), FileAccess.Write, share)
	  {
	    Init(path, maxFileLength, mode);
	  }

	  public FileStreamWithBackup(string path, long maxFileLength, FileMode mode, FileShare share, int bufferSize)
	    : base(path, BaseFileMode(mode), FileAccess.Write, share, bufferSize)
	  {
	    Init(path, maxFileLength, mode);
	  }

	  public FileStreamWithBackup(string path, long maxFileLength, FileMode mode, FileShare share, int bufferSize, bool isAsync)
	    : base(path, BaseFileMode(mode), FileAccess.Write, share, bufferSize, isAsync)
	  {
	    Init(path, maxFileLength, mode);
	  }
	  #endregion

	  #region Properties
	  private long _MaxFileLength;
	  private bool _CanSplitData;
	  #endregion

	  #region Properties Methods
	  public long MaxFileLength 
	  { 
	    get { return _MaxFileLength; }
	  }
	  public bool CanSplitData
	  { 
	    get { return _CanSplitData; } 
	    set { _CanSplitData = value; } 
	  }
	  #endregion

	  #region Overrided Properties Methods
	  public override bool CanRead { get { return false; } }
	  #endregion

	  #region Overrided Public Members
	  public override void Write(byte[] array, int offset, int count)
	  {
	    try
	    {
        lock (this)
        {
          int actualCount = System.Math.Min(count, array.Length);

          if (Position + actualCount <= _MaxFileLength)
          {
            base.Write(array, offset, count);
          }
          else
          {
            if (CanSplitData)
            {
              int partialCount = (int)(System.Math.Max(_MaxFileLength, Position) - Position);
              base.Write(array, offset, partialCount);

              offset += partialCount;
              count = actualCount - partialCount;
            }
            else
            {
              if (count > _MaxFileLength)
                throw new ArgumentOutOfRangeException("Buffer size exceeds maximum file length");
            }

            //Backup And Reset Stream
            string backupFileName = GetBackupFileName();
            base.Flush();
            File.Copy(Name, backupFileName, true);
            base.SetLength(0);

            base.Write(array, offset, count);
          }
        }
	    }
	    catch(Exception ex)
	    {
  		  throw ex;
  	  }
	  }
	  #endregion

	  #region Private Members
	  private void Init(string path, long maxFileLength, FileMode mode)
	  {
	    try
	    {
	      if( maxFileLength <= 0 )
	        throw new ArgumentOutOfRangeException("Invalid maximum file length");

	      _MaxFileLength = maxFileLength;
	      _CanSplitData = true;

	      switch(mode)
	      {
	        case FileMode.Create: 
	        case FileMode.CreateNew:
	        case FileMode.Truncate:
	        default:
			      Seek(0, SeekOrigin.End);
		      break;
	      }
	    }
	    catch(Exception ex) 
	    {
  		  throw ex;
	    }
	  }

	  private string GetBackupFileName()
	  {
	    string backUpFileName;

	    try
	    {
		    DateTime dtNow = DateTime.Now; 

		    backUpFileName=Name+"."
		              +String.Format("{0:0000}",dtNow.Year)
		              +String.Format("{0:00}",dtNow.Month)
		              +String.Format("{0:00}",dtNow.Day)
		              +"."
		              +String.Format("{0:00}",dtNow.Hour)
		              +String.Format("{0:00}",dtNow.Minute)
		              +String.Format("{0:00}",dtNow.Second)
		              +".OLD";    

        return backUpFileName;
	    }
	    catch(Exception ex)
	    {
		    throw ex;
	    }
	  }

	  private static FileMode BaseFileMode(FileMode mode)
	  {
	    return mode == FileMode.Append ? FileMode.OpenOrCreate : mode;
	  }
	  #endregion
  }
}
