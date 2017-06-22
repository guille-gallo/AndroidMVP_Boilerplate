package assist.com.appmetrics;

import android.content.Context;
import android.support.annotation.NonNull;
import android.support.annotation.Nullable;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ArrayAdapter;
import android.widget.Filterable;
import android.widget.TextView;

import java.util.ArrayList;
import java.util.List;

/**
 * Created by ggallo on 15/6/2017.
 */

public class UserDetailAdapter extends ArrayAdapter implements Filterable {

    private List<UserDetail> items;
    private int R_layout_IdView;
    private Context context;


    public UserDetailAdapter(Context context, int R_layout_IdView, ArrayList<UserDetail> items) {
        super(context, R_layout_IdView);
        this.context = context;
        this.items = items;
        this.R_layout_IdView = R_layout_IdView;
    }

    @Override
    public int getCount() {
        return items.size();
    }

    @Override
    public Object getItem(int position) {
        return items.get(position);
    }

    @Override
    public long getItemId(int position) {
        return position;
    }


    @NonNull
    @Override
    public View getView(int position, @Nullable View convertView, @NonNull ViewGroup parent) {
        //return super.getView(position, convertView, parent);

        if (convertView == null) {
            LayoutInflater vi = (LayoutInflater) context.getSystemService(Context.LAYOUT_INFLATER_SERVICE);
            convertView = vi.inflate(R_layout_IdView, null);
        }
        onEntrada (items.get(position), convertView);
        return convertView;
    }


    public void onEntrada (final Object item, View view){
        if (item != null) {
            //FullName
            TextView txtFullName = (TextView) view.findViewById(R.id.userName);
            txtFullName.setText(((UserDetail) item).getUserName());

            TextView txtFirstName = (TextView) view.findViewById(R.id.userFirstName);
            txtFirstName.setText(((UserDetail) item).getUserFirstName());

            TextView txtLastName = (TextView) view.findViewById(R.id.userLastName);
            txtLastName.setText(((UserDetail) item).getUserLastName());
        }
    }


}