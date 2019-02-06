function attachEvents() {   // PUT THIS TO MY FISHSPINDAYS PROJECT !!!
// paris,fr ; London, uk ; or only Paris ; Landon ; Sofia
    $('#submit').on('click', getWeather);

    let result = $('<div>').attr('id', 'result');
    $('#request').append(result);

    function getWeather() {
        let requestedLocation = $('#location').val();
		let corsURL = 'https://cors-anywhere.herokuapp.com/';
        let apiCall = corsURL + `http://api.openweathermap.org/data/2.5/weather?q=${requestedLocation}&APPID=48090034853e62416ea8a8b211062132`;
        $.getJSON(apiCall, weatherCallBack);

        function weatherCallBack(weatherData) {

            console.log(weatherData);

            let cityName = weatherData.name;
            let country = weatherData.sys.country;
            let description = weatherData.weather[0].description;
            let temp = Number(weatherData.main.temp) - 273.15;

            result.html(`<p>${cityName} / ${country} / ${temp.toFixed(0)}&#176; / ${description}</p>`);
        }

        $('#location').val('');
    }
}