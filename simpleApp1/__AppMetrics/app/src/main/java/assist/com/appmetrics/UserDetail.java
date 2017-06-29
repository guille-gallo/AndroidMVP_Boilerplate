package assist.com.appmetrics;

/**
 * Created by ggallo on 15/6/2017.
 */

public class UserDetail {

    private String userName;
    private String userFirstName;
    private String userLastName;

    public UserDetail(String userName, String userFirstName,String userLastName) {
        this.userName = userName;
        this.userFirstName = userFirstName;
        this.userLastName = userLastName;
    }

    // userName
    public String getUserName() {
        return userName;
    }
    public void setUserName(String userName) {
        this.userName = userName;
    }

    // userFirstName
    public String getUserFirstName() {
        return userFirstName;
    }
    public void setUserFirstName(String userFirstName) {
        this.userFirstName = userFirstName;
    }

    // userLastName
    public String getUserLastName() {
        return userLastName;
    }
    public void setUserLastName(String userLastName) {
        this.userLastName = userLastName;
    }

}
