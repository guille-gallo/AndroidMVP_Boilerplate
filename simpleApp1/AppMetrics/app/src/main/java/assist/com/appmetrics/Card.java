package assist.com.appmetrics;

import org.json.JSONArray;

/**
 * Created by ggallo on 13/6/2017.
 */

public class Card {

    private String cardTitle;
    //private String[] usersOnline;
    private JSONArray usersOnline;
    private String usersLastHalfHour;

    public Card(String cardTitle, JSONArray usersOnline, String usersLastHalfHour) {
        this.cardTitle = cardTitle;
        this.usersOnline = usersOnline;
        this.usersLastHalfHour = usersLastHalfHour;
    }

    public String getCardTitle() {
        return cardTitle;
    }

    public JSONArray getOnlineUsers() {
        return usersOnline;
    }

    public String getUsersLastHalfHour() {
        return usersLastHalfHour;
    }


    public void setCardTitle(String cardTitle) {
        this.cardTitle = cardTitle;
    }
}
