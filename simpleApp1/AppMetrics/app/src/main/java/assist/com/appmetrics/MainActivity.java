package assist.com.appmetrics;

import android.graphics.drawable.AnimationDrawable;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.support.v7.widget.LinearLayoutManager;
import android.support.v7.widget.RecyclerView;
import android.util.Log;
import android.view.View;
import android.widget.ImageView;
import com.android.volley.NetworkResponse;
import com.android.volley.Request;
import com.android.volley.Response;
import com.android.volley.VolleyError;
import com.android.volley.toolbox.JsonArrayRequest;
import com.android.volley.toolbox.JsonObjectRequest;
import com.google.firebase.iid.FirebaseInstanceId;
import com.google.firebase.iid.FirebaseInstanceIdService;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;
import java.util.ArrayList;
import java.util.List;
import assist.com.appmetrics.restclient.VolleyClient;
import static android.content.ContentValues.TAG;

public class MainActivity extends AppCompatActivity {

    private List<Card> cardList;
    private CardAdapter adapter;
    private static final String URL_BASE_DEMO = "https://services.assistsa.com.ar/";

    private AnimationDrawable animationDrawable;
    //public String[] usersArray = new String[0];

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
     * Avoid cards duplication when hitting back button from other activity.
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

    public void initRequest() {
        // Init ProgressBar
        ImageView mProgressBar = (ImageView) findViewById(R.id.main_progress);
        mProgressBar.setBackgroundResource(R.drawable.circular_progressbar);
        animationDrawable = (AnimationDrawable)mProgressBar.getBackground();
        mProgressBar.setVisibility(View.VISIBLE);
        animationDrawable.start();

        //String url = "https://api.myjson.com/bins/kcxx3";

        String url = URL_BASE_DEMO + "SWCFAppMetrics/Services/Mobile.svc/GetCardInfo";
        JsonObjectRequest jsonObjectRequest = new JsonObjectRequest (Request.Method.GET, url, null, new Response.Listener<JSONObject>() {
            @Override
            public void onResponse(JSONObject response) {
                    try {
                        JSONArray usersOnline = response.getJSONArray("items");
                        cardList.add(new Card("TG", usersOnline, "1"));
                    } catch (JSONException ex) {
                        System.out.println(ex);
                    }
                hideProgressBar();
            }
        }, new Response.ErrorListener() {
            @Override
            public void onErrorResponse(VolleyError error) {
                String hola = error.toString();
            }
        });

        // Access the RequestQueue through your singleton class.
        VolleyClient.getInstance(this).addToRequestQueue(jsonObjectRequest);
    }

    public void hideProgressBar() {
        ImageView mProgressBar = (ImageView) findViewById(R.id.main_progress);
        mProgressBar.setVisibility(View.GONE);
        animationDrawable.stop();
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
