package assist.com.appmetrics;

/**
 * Created by ggallo on 13/6/2017.
 */

public class Card {

    private String cardTitle;
    private String usersOnline;
    private String usersLastHalfHour;

    public Card(String cardTitle, String usersOnline, String usersLastHalfHour) {
        this.cardTitle = cardTitle;
        this.usersOnline = usersOnline;
        this.usersLastHalfHour = usersLastHalfHour;
    }

    public String getCardTitle() {
        return cardTitle;
    }

    public String getOnlineUsers() {
        return usersOnline;
    }

    public String getUsersLastHalfHour() {
        return usersLastHalfHour;
    }


    public void setCardTitle(String cardTitle) {
        this.cardTitle = cardTitle;
    }
}
