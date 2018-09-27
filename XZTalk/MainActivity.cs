using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Widget;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Xamarin.Database;
using System.Collections.Generic;


namespace XZTalk
{
    [Activity(Label = "ChatAppUsingFirebase", MainLauncher = true, Theme = "@style/Theme.AppCompat.Light.NoActionBar")]
    public class MainActivity : AppCompatActivity, IValueEventListener
    {
        private FirebaseClient firebaseClient;
        private List<MessageContent> lstMessage = new List<MessageContent>();
        private ListView lstChat;
        private EditText edtChat;
        private FloatingActionButton fab;
        private DatabaseReference myref;
        private FirebaseDatabase database;

        public int MyResultCode = 1;

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {

            base.OnCreate(savedInstanceState);
            FirebaseOptions.Builder builder  = new FirebaseOptions.Builder();
            builder.SetApiKey("AIzaSyAe9KR6oAI5yZ6Y3RsR2-I_VWCAg4ut4ek");
            builder.SetApplicationId("1:471441222469:android:fcf08d13fa42ec25");
            builder.SetDatabaseUrl(GetString(Resource.String.firebase_url));
            builder.SetProjectId("xztalk-51c5e");

            FirebaseOptions firebaseopt = builder.Build() ;
            FirebaseApp firebaseapp = FirebaseApp.InitializeApp(this,firebaseopt);
            database = FirebaseDatabase.GetInstance(firebaseapp);
            // Set our view from the "main" layout resource  
            SetContentView(Resource.Layout.main);
            
            
            
            fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            edtChat = FindViewById<EditText>(Resource.Id.input);
            lstChat = FindViewById<ListView>(Resource.Id.list_of_messages);

            fab.Click += delegate { PostMessage(); };

            if (FirebaseAuth.Instance.CurrentUser == null)
                StartActivityForResult(new Android.Content.Intent(this, typeof(SignIn)), MyResultCode);
            else
            {
                Toast.MakeText(this, "Welcome" + FirebaseAuth.Instance.CurrentUser.Email, ToastLength.Short).Show();
                DisplayChatMessage();
            }
        }

        private  void PostMessage()
        {
            myref = database.GetReference("user");
            myref.SetValue(edtChat.Text);//, FirebaseAuth.Instance.CurrentUser.Email);
           // var items = await firebaseClient.Child("chats").PostAsync(new MessageContent(FirebaseAuth.Instance.CurrentUser.Email, edtChat.Text));
            edtChat.Text = "";
        }
        public void OnCancelled(DatabaseError error)
        {

        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            DisplayChatMessage();
        }

        private async void DisplayChatMessage()
        {
            lstMessage.Clear();
            var items = await firebaseClient.Child("chats")
                .OnceAsync<MessageContent>();
            foreach (var item in items)
                lstMessage.Add(item.Object);
            ListViewAdapter adapter = new ListViewAdapter(this, lstMessage);
            lstChat.Adapter = adapter;
        }
    }
}