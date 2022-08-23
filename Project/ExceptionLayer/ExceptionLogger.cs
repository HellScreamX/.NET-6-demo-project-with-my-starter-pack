using Microsoft.EntityFrameworkCore;
using Project.DAL;

public static class DbContextExtensions
{
  public static void Reset(this DbContext context)
  {
      var entries = context.ChangeTracker
                           .Entries()
                           .Where(e => e.State != EntityState.Unchanged)
                           .ToArray();

      foreach (var entry in entries)
      {
          switch (entry.State)
          {
              case EntityState.Modified:
                  entry.State = EntityState.Unchanged;
                  break;
              case EntityState.Added:
                  entry.State = EntityState.Detached;
                  break;
              case EntityState.Deleted:
                  entry.Reload();
                  break;
          }
      }
  }
}
public interface IExceptionLogger{
    void Log(Exception exception, ExceptionHttpContextInput httpLog, string errorCode);
    List<ExceptionLog> GetList();
}
public class ExceptionLogger:IExceptionLogger{
    private ApplicationDbContext _context;
    public ExceptionLogger( ApplicationDbContext context){
        _context = context;
    }
    public void Log(Exception exception,ExceptionHttpContextInput httpLog, string errorCode){
     
     DbContextExtensions.Reset(_context);

         _context.ExceptionLogs.Add(
             new ExceptionLog{
                 Action=exception.TargetSite!.Name,
                 Message=exception.Message,
                 Type=exception.GetType().Name,
                 Date=DateTime.Now,
                 userId=httpLog.userId,
                 host=httpLog.host,
                 path=httpLog.path,
                 method=httpLog.method,
                 errorCode=errorCode
             }
         );
         _context.SaveChanges();
    }
    public List<ExceptionLog> GetList(){
        var result = _context.ExceptionLogs.ToList();
        return result;
    }
}