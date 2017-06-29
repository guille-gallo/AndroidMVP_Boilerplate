package assist.com.appmetrics;

import android.animation.ObjectAnimator;
import android.os.Bundle;
import android.support.v7.app.AppCompatActivity;
import android.util.Log;
import android.view.Menu;
import android.view.MenuInflater;
import android.view.View;
import android.view.animation.DecelerateInterpolator;
import android.widget.ListView;
import android.widget.ProgressBar;
import com.android.volley.Request;
import com.android.volley.Response;
import com.android.volley.VolleyError;
import com.android.volley.toolbox.JsonArrayRequest;
import org.json.JSONArray;
import org.json.JSONException;
import java.util.ArrayList;
import assist.com.appmetrics.restclient.VolleyClient;

/**
 * Created by ggallo on 14/6/2017.
 */

public class CardActivity extends AppCompatActivity {

    ArrayList<UserDetail> datos = new ArrayList<>();
    UserDetailAdapter adapter;
    public ListView lista;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_card);

        adapter = new UserDetailAdapter(this, R.layout.user_item_view, datos);
        lista = (ListView) findViewById(R.id.lv_users);
        lista.setAdapter(adapter);

        /**
         * AppBar configuration
         */
        setTitle("Usuarios conectados");
        getSupportActionBar().setHomeButtonEnabled(true);
        getSupportActionBar().setDisplayHomeAsUpEnabled(true);
    }

    @Override
    public void onResume() {
        super.onResume();
        initRequest();
    }

    public void initRequest() {
        // Init ProgressBar
        ProgressBar mprogressBar = (ProgressBar) findViewById(R.id.circular_progress_bar);
        ObjectAnimator anim = ObjectAnimator.ofInt(mprogressBar, "progress", 0, 100);
        anim.setInterpolator(new DecelerateInterpolator());
        anim.start();

        String url = "https://api.myjson.com/bins/7bukn";
        JsonArrayRequest jsArrayRequest = new JsonArrayRequest (Request.Method.GET, url, null, new Response.Listener<JSONArray>() {
            @Override
            public void onResponse(JSONArray response) {
                for (int i = 0; i < response.length(); i++) {
                    try {
                        String userName = response.getJSONObject(i).getString("userName");
                        String userFirstName = response.getJSONObject(i).getString("userFirstName");
                        String userLastName = response.getJSONObject(i).getString("userLastName");

                        datos.add(new UserDetail(userName, userFirstName, userLastName));

                    } catch (JSONException ex) {
                        System.out.println(ex);
                    }
                }
                adapter.notifyDataSetChanged();
                hideProgressBar();
            }
        }, new Response.ErrorListener() {
            @Override
            public void onErrorResponse(VolleyError error) {
                String hola = error.toString();
                // TODO Auto-generated method stub
            }
        });
        // Access the RequestQueue through your singleton class.
        VolleyClient.getInstance(this).addToRequestQueue(jsArrayRequest);
    }

    public void hideProgressBar() {
        ProgressBar mprogressBar = (ProgressBar) findViewById(R.id.circular_progress_bar);
        mprogressBar.setVisibility(View.INVISIBLE);
    }

    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        MenuInflater inflater = getMenuInflater();
        inflater.inflate(R.menu.main, menu);

        /*final MenuItem item = menu.findItem(R.id.menu_search);
        final SearchView searchView = (SearchView) item.getActionView();
        searchView.setQueryHint("Buscar...");

        searchView.setOnQueryTextListener(new SearchView.OnQueryTextListener() {
            @Override
            public boolean onQueryTextSubmit (String arg0) {
                return false;
            }

            @Override
            public boolean onQueryTextChange (String query) {
                adapter.getFilter().filter(query);

                return false;
            }
        });*/
        return super.onCreateOptionsMenu(menu);
    }
}
