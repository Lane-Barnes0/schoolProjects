

const vm = Vue.createApp({

	 data() {
                return {
                    title : "Vue.JS Weather App",
                    location : undefined,
                    latitude : undefined, 
                    longitude : undefined,
                    todo : undefined,
                    temp : undefined,
                    high : undefined,
                    low : undefined,
                    weather : undefined,
                    humidity : undefined,
                    pressure : undefined,
                    date : new Date().toLocaleDateString("en-US") + ", " + new Date().toLocaleTimeString("en-US"),
                    fiveDayForcast : [

                    ],
                    unlikely : 0,
                    neutral : 40,
                    likely : 0,
                            
    
                }
            },

                methods: {
                    pluralize(n) {
                        return n == 1 ? ' ' : 's';
                    }, 

                    cycle(ev) {
						let color = ev.target.getAttribute('class');
						if (color === 'neutral stuff-box' ) {
							ev.target.setAttribute('class' , "unlikely stuff-box")
							this.unlikely += 1
							this.neutral -= 1
						}

					if (color === 'unlikely stuff-box' ) {
							ev.target.setAttribute('class' , "likely stuff-box")
							this.likely += 1
							this.unlikely -= 1
						}

					if (color === 'likely stuff-box' ) {
							ev.target.setAttribute('class' , "neutral stuff-box")
							this.neutral += 1
							this.likely -= 1
						}
                        
                    }
                  
                },

                computed: {
                   

                },

                created() {
                	fetch('http://api.ipstack.com/check?access_key=c199e3f536074fca39a4dfd0b6a1abb5')
						.then(response => response.json())
						.then(json => {
							this.latitude = json.latitude
							this.longitude = json.longitude
							this.location = json.city + ", " + json.region_name + ", " + json.country_name;
							return fetch(`http://api.openweathermap.org/data/2.5/weather?lat=${json.latitude}&lon=${json.longitude}&appid=a9c0e2832739ed027c3af80a9fecbdab&units=imperial`)
						})
						.then(response => response.json())
						.then(json => {
							this.temp = json.main.temp
							this.high = json.main.temp_max
							this.low = json.main.temp_min
							this.weather = json.weather[0].description
							this.humidity = json.main.humidity
							this.pressure = json.main.pressure

							return fetch(`http://api.openweathermap.org/data/2.5/forecast?lat=${this.latitude}&lon=${this.longitude}&appid=a9c0e2832739ed027c3af80a9fecbdab&units=imperial`)

						})
						.then(response => response.json()) 
						.then(json => {
							this.fiveDayForcast = json.list
						});

					
                }

            }).mount('#app');
