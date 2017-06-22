package assist.com.appmetrics;

import android.animation.ObjectAnimator;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.support.v7.widget.LinearLayoutManager;
import android.support.v7.widget.RecyclerView;
import android.util.Log;
import android.view.View;
import android.view.animation.DecelerateInterpolator;
import android.widget.ProgressBar;
import com.android.volley.Request;
import com.android.volley.Response;
import com.android.volley.VolleyError;
import com.android.volley.toolbox.JsonArrayRequest;
import com.google.firebase.iid.FirebaseInstanceId;
import com.google.firebase.iid.FirebaseInstanceIdService;
import org.json.JSONArray;
import org.json.JSONException;
import java.util.ArrayList;
import java.util.List;

import assist.com.appmetrics.restclient.VolleyClient;

import static android.content.ContentValues.TAG;

public class MainActivity extends AppCompatActivity {

    private List<Card> cardList;
    private CardAdapter adapter;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        final RecyclerView mRecyclerView;
        final RecyclerView.LayoutManager mLayoutManager;

        mRecyclerView = (RecyclerView) findViewById(R.id.my_recycler_view);

        // use this setting to improve performance if you know that changes
        // in content do not change the layout size of the RecyclerView
        mRecyclerView.setHasFixedSize(true);

        // use a linear layout manager
        mLayoutManager = new LinearLayoutManager(this);
        mRecyclerView.setLayoutManager(mLayoutManager);

        cardList = new ArrayList<>();
        adapter = new CardAdapter(this, cardList);
        mRecyclerView.setAdapter(adapter);
    }

    /**
     * This was added to avoid cards duplication when hitting back button from other activity.
     */
    @Override
    public void onRestart() {
        super.onRestart();
        startActivity(getIntent());
    }

    @Override
    public void onResume() {
        super.onResume();
        initRequest();
    }

    private void initRequest() {
        // Init ProgressBar
        ProgressBar mprogressBar = (ProgressBar) findViewById(R.id.circular_progress_bar_main);
        ObjectAnimator anim = ObjectAnimator.ofInt(mprogressBar, "progress", 0, 100);
        anim.setInterpolator(new DecelerateInterpolator());
        anim.start();

        String url = "https://api.myjson.com/bins/kcxx3";
        JsonArrayRequest jsArrayRequest = new JsonArrayRequest (Request.Method.GET, url, null, new Response.Listener<JSONArray>() {
            @Override
            public void onResponse(JSONArray response) {
                for (int i = 0; i < response.length(); i++) {
                    try {
                        String cardTitle = response.getJSONObject(i).getString("cardTitle");
                        String usersOnline = response.getJSONObject(i).getString("usersOnline");
                        String usersLastHalfHour = response.getJSONObject(i).getString("usersLastHalfHour");

                        cardList.add(new Card(cardTitle, usersOnline, usersLastHalfHour));

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
        ProgressBar mprogressBar = (ProgressBar) findViewById(R.id.circular_progress_bar_main);
        mprogressBar.setVisibility(View.INVISIBLE);
    }


    public static class MyFirebaseInstanceIDService extends FirebaseInstanceIdService {

        @Override
        public void onTokenRefresh() {
            // Get updated InstanceID token.
            String refreshedToken = FirebaseInstanceId.getInstance().getToken();
            Log.d(TAG, "*** *** *** *** *** Refreshed token: " + refreshedToken);

            // If you want to send messages to this application instance or
            // manage this apps subscriptions on the server side, send the
            // Instance ID token to your app server.

            //sendRegistrationToServer(refreshedToken);
        }

    }


}
