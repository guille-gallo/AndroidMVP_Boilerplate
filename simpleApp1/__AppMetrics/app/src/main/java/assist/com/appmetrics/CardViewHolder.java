package assist.com.appmetrics;

import android.support.v7.widget.RecyclerView;
import android.view.View;
import android.widget.ImageView;
import android.widget.TextView;

/**
 * Created by ggallo on 13/6/2017.
 */

public class CardViewHolder extends RecyclerView.ViewHolder {
    public ImageView cardTitle;
    public TextView usersOnline;
    public TextView usersLastHalfHour;

    public CardViewHolder(View view) {
        super(view);
        cardTitle = (ImageView) view.findViewById(R.id.img_logo);
        usersOnline = (TextView) view.findViewById(R.id.users_online);
        usersLastHalfHour = (TextView) view.findViewById(R.id.users_halfhour);
    }
}
