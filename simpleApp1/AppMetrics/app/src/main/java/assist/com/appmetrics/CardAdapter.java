package assist.com.appmetrics;

import android.content.Context;
import android.content.Intent;
import android.content.res.Resources;
import android.graphics.drawable.Drawable;
import android.support.v4.content.res.ResourcesCompat;
import android.support.v7.widget.RecyclerView;
import android.text.Layout;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import java.util.List;

/**
 * Created by ggallo on 13/6/2017.
 */

public class CardAdapter extends RecyclerView.Adapter<CardViewHolder>{

    private List<Card> cardList;
    private Context mContext;

    CardAdapter(Context mContext, List<Card> cardList) {
        this.mContext = mContext;
        this.cardList = cardList;
    }

    @Override
    public CardViewHolder onCreateViewHolder(ViewGroup parent, int viewType) {

        View itemView = LayoutInflater.from(parent.getContext())
                .inflate(R.layout.cards_view, parent, false);

        return new CardViewHolder(itemView);
    }

    @Override
    public void onBindViewHolder(final CardViewHolder holder, int position) {
        Card card = cardList.get(position);
        Boolean hasCardTitle = !card.getCardTitle().isEmpty();
        String cardTitleID = card.getCardTitle();
        Integer appIcon = R.drawable.ic_logo_tg;

        if (hasCardTitle) {
            switch (cardTitleID) {
                case ("TG") :
                    appIcon = R.drawable.ic_logo_tg;
                    break;
                case ("GG") :
                    appIcon = R.drawable.ic_logo_gg;
                    break;
                case ("CEQ") :
                    appIcon = R.drawable.ic_logo_ceq;
                    break;
            }
        }

        Context elContesto = holder.cardTitle.getContext();
        Drawable elDraguable = elContesto.getResources().getDrawable(appIcon);

        holder.cardTitle.setImageDrawable(elDraguable);
        holder.usersOnline.setText(card.getOnlineUsers());
        holder.usersLastHalfHour.setText(card.getUsersLastHalfHour());
        holder.cardTitle.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                Intent intent = new Intent(mContext, CardActivity.class);
                mContext.startActivity(intent);
            }
        });
    }

    @Override
    public int getItemCount() {
        return cardList.size();
    }
}
