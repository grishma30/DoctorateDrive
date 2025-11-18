document.addEventListener('DOMContentLoaded', function () {
    // Change Email
    document.getElementById('changeEmailBtn').onclick = function () {
        document.getElementById('emailOtpSection').style.display = 'inline';
    };
    document.getElementById('sendEmailOtpBtn').onclick = function () {
        let newEmail = document.getElementById('newEmail').value;
        fetch('/Home/SendOtp', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ EmailOrMobile: newEmail })
        }).then(r => r.json()).then(res => {
            document.getElementById('emailOtpStatus').innerText = res.message;
        });
    };
    document.getElementById('verifyEmailOtpBtn').onclick = function () {
        // call your API to verify OTP and update email in backend, then on success:
        // update displayed email
        let newEmail = document.getElementById('newEmail').value;
        document.getElementById('studentEmail').innerText = newEmail;
        document.getElementById('emailOtpSection').style.display = 'none';
    };

    // Change Mobile
    document.getElementById('changeMobileBtn').onclick = function () {
        document.getElementById('mobileOtpSection').style.display = 'inline';
    };
    document.getElementById('sendMobileOtpBtn').onclick = function () {
        let newMobile = document.getElementById('newMobile').value;
        fetch('/Home/SendOtp', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ EmailOrMobile: newMobile })
        }).then(r => r.json()).then(res => {
            document.getElementById('mobileOtpStatus').innerText = res.message;
        });
    };
    document.getElementById('verifyMobileOtpBtn').onclick = function () {
        // call your API to verify OTP and update mobile in backend, then on success:
        // update displayed mobile
        let newMobile = document.getElementById('newMobile').value;
        document.getElementById('studentMobile').innerText = newMobile;
        document.getElementById('mobileOtpSection').style.display = 'none';
    };
});
