/**
 *  Interlogix NetworX NX-595E
 *
 *  Copyright 2018 Simon Hebert (shebert@cbti.net).
 *
 *  Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except
 *  in compliance with the License. You may obtain a copy of the License at:
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 *  Unless required by applicable law or agreed to in writing, software distributed under the License is distributed
 *  on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License
 *  for the specific language governing permissions and limitations under the License.
 */
metadata {
	definition (name: "Interlogix NetworX NX-595E", namespace: "simonhebert", author: "Simon Hebert") {
		//capability "Actuator"
		//capability "Alarm"
		capability "Alarm System"
		capability "Configuration"
        //capability "Polling"
        capability "Refresh"
		capability "Security System"
		//capability "Sensor"
        
        // alarmSystemStatus[off, stay, away]
        // securitySystemStatus[off, stay, away]
        // alarm
        attribute "alarmStatus", "JSON_OBJECT"
        attribute "isSystemReady", "BOOLEAN"
        attribute "isFireAlarm", "BOOLEAN"
        attribute "isIntrusionAlarm", "BOOLEAN"
        attribute "isExitTimeDelay", "BOOLEAN"
        attribute "isEntryTimeDelay", "BOOLEAN"
        attribute "isZoneBypassEnabled", "BOOLEAN"
        attribute "isChimeEnabled", "BOOLEAN"
        attribute "systemStatus", "STRING"
        attribute "zones", "JSON_OBJECT"

        // configure
        // refresh
		// armStay
        // armAway
        // disarm
        command "chimeOn"
        command "chimeOff"
	}

    preferences {
        input name: "ipAddress", type: "text", title: "IP Address", description: "", required: false, displayDuringSetup: true//, defaultValue: '192.168.2.16'
        input name: "portNumber", type: "number", title: "Port Number", description: "", required: false, displayDuringSetup: true//, defaultValue: '595'
    }
    
	simulator {
		// TODO: define status and reply messages here
	}

	tiles(scale: 2) {
		// TODO: define your main and details tiles here
        /*
        Alarm related icons:
        st.alarm.alarm.alarm
        st.alarm.beep.beep
        st.alarm.smoke.smoke
        st.security.alarm.alarm
		st.security.alarm.clear
        st.security.alarm.off
        st.security.alarm.on
        st.security.alarm.partial
		*/
        
        multiAttributeTile(name:"alarmSystemTile", type:"generic", width:6, height:4) {
            tileAttribute("device.alarmSystemStatus", key: "PRIMARY_CONTROL") {
                attributeState "off", label:'${name}', action: "armAway", icon: "st.security.alarm.off", backgroundColor:"#ffffff", nextState:"turningAway"
                attributeState "away", label:'${name}', action: "disarm", icon: "st.security.alarm.on", backgroundColor:"#bc2323", nextState:"turningOff"
                attributeState "stay", label: '${name}', action: "disarm", icon: "st.security.alarm.partial", backgroundColor: "#f1d801", nextState: "turningOff"
                attributeState "turningAway", label:"Turning away", icon:"st.security.alarm.on", backgroundColor:"#bc2323", nextState:"turningOff"
                attributeState "turningOff", label:"Turning off", icon:"st.security.alarm.on", backgroundColor:"#ffffff", nextState:"turningAway"
            }
            tileAttribute("device.systemStatus", key: "SECONDARY_CONTROL") {
                attributeState "systemStatus", label:'${currentValue}', defaultState: true
            }
		}

		standardTile("armAwayTile", "device.alarmSystemStatus", width: 2, height: 2, decoration: "flat", canChangeIcon: true) {
            state "off", label: "Away", action: "armAway", icon: "st.security.alarm.off", backgroundColor: "#ffffff", defaultState: true, nextState: "turningAway"
            state "away", label: "Away", action: "disarm", icon: "st.security.alarm.on", backgroundColor: "#bc2323", nextState: "turningOff"
            state "turningAway", label:"Turning away", icon:"st.security.alarm.on", backgroundColor:"#bc2323", nextState: "turningOff"
            state "turningOff", label:"Turning off", icon:"st.security.alarm.off", backgroundColor:"#ffffff", nextState: "turningAway"
        }
        
		standardTile("armStayTile", "device.alarmSystemStatus", width: 2, height: 2, decoration: "flat", canChangeIcon: true) {
            state "off", label: "Stay", action: "armStay", icon: "st.security.alarm.off", backgroundColor: "#ffffff", defaultState: true, nextState: "turningStay"
            state "stay", label: "Stay", action: "disarm", icon: "st.security.alarm.partial", backgroundColor: "#f1d801", nextState: "turningOff"
            state "turningStay", label:"Turning stay", icon:"st.security.alarm.partial", backgroundColor:"#f1d801", nextState: "turningOff"
            state "turningOff", label:"Turning off", icon:"st.security.alarm.off", backgroundColor:"#ffffff", nextState: "turningStay"
        }

/*
        valueTile("systemReadyTile", "device.isSystemReady", width: 2, height: 2, decoration: "flat") {
            state("isSystemReady", label:'${currentValue}', backgroundColor: "#cccccc",
            	backgroundColors:[
                    [value: "true", color: "#00a0dc"],
                    [value: "false", color: "#ffffff"]
                ]
            )
        }
*/
        standardTile("systemReadyTile", "device.isSystemReady", width: 2, height: 2, decoration: "flat") {
            state("false", label:"System Ready", backgroundColor: "#cccccc", defaultState: true)
            state("true", label:"System Ready", backgroundColor: "#44b621")
        }
        
        standardTile("intrusionAlarmTile", "device.isIntrusionAlarm", width: 2, height: 2, decoration: "flat") {
            state("false", label:"Intrusion Alarm", backgroundColor: "#cccccc", defaultState: true)
            state("true", label:"Intrusion Alarm", backgroundColor: "#e86d13")
        }

        standardTile("fireAlarmTile", "device.isFireAlarm", width: 2, height: 2, decoration: "flat") {
            state("false", label:"Fire Alarm", backgroundColor: "#cccccc", defaultState: true)
            state("true", label:"Fire Alarm", backgroundColor: "#e86d13")
        }
       
        standardTile("chimeTile", "device.isChimeEnabled", width: 2, height: 2, decoration: "flat", canChangeIcon: true) {
            state "false", label: "Chime", action: "chimeOn", icon: "st.quirky.spotter.quirky-spotter-sound-off", backgroundColor: "#ffffff", defaultState: true, nextState: "turningOn"
            state "true", label: "Chime", action: "chimeOff", icon: "st.alarm.beep.beep", backgroundColor: "#00a0dc", nextState: "turningOff"
            state "turningOn", label:"Turning on", icon:"st.alarm.beep.beep", backgroundColor:"#00a0dc", nextState: "turningOff"
            state "turningOff", label:"Turning off", icon:"st.quirky.spotter.quirky-spotter-sound-off", backgroundColor:"#ffffff", nextState: "turningOn"
        }
        
        standardTile("exitTimeDelayTile", "device.isExitTimeDelay", width: 2, height: 2, decoration: "flat") {
            state("false", label:"Exit Time Delay", backgroundColor: "#cccccc", defaultState: true)
            state("true", label:"Exit Time Delay", backgroundColor: "#00a0dc")
        }
        
        standardTile("entryTimeDelayTile", "device.isEntryTimeDelay", width: 2, height: 2, decoration: "flat") {
            state("false", label:"Entry Time Delay", backgroundColor: "#cccccc", defaultState: true)
            state("true", label:"Entry Time Delay", backgroundColor: "#00a0dc")
        }
        
        standardTile("zoneBypassEnabledTile", "device.isZoneBypassEnabled", width: 2, height: 2, decoration: "flat") {
            state("false", label:"Zone Bypass", backgroundColor: "#cccccc", defaultState: true)
            state("true", label:"Zone Bypass", backgroundColor: "#00a0dc")
        }
 
        standardTile("refreshTile", "device.refresh", width: 2, height: 2, decoration: "flat") {
            state "refresh", label:"Refresh", action:"refresh", icon:"st.secondary.refresh"
        }
            
        // The "switchTile" will be main tile, displayed in the "Things" view
        main("alarmSystemTile")

        // the "switch" and "power" tiles will appear in the Device Details
        // view (order is left-to-right, top-to-bottom)
        details(["alarmSystemTile", "armAwayTile", "armStayTile", "systemReadyTile", "intrusionAlarmTile", "fireAlarmTile", "chimeTile", "exitTimeDelayTile", "entryTimeDelayTile", "zoneBypassEnabledTile", "refreshTile"])
	}
}

def installed() {
	log.debug "Executing 'installed'"
	initialize()
}

def updated() {
	log.debug "Executing 'updated'"
    initialize()
}

// NOT USED I THINK, test it
def initialized() {
	log.debug "Executing 'initialized'"
}

def initialize() {
	log.debug "Executing 'initialize'"

	unschedule()
    
    //schedule("5 * * * * ?", myCommand) // Min. 1 minute limited by the system
    runEvery1Minute(getAlarmStatus)
}

// parse events into attributes
def parse(String description) {
	log.debug "Parsing '${description}'"
    
	// TODO: handle 'alarmSystemStatus' attribute
	// TODO: handle 'securitySystemStatus' attribute
	// TODO: handle 'alarm' attribute
  
	def msg = parseLanMessage(description)
    
    log.debug "Msg: " + msg
    
    def headersAsString = msg.header // => headers as a string
    def headerMap = msg.headers      // => headers as a Map
    def body = msg.body              // => request body as a string
    def status = msg.status          // => http status code of the response
    def json = msg.json              // => any JSON included in response body, as a data structure of lists and maps
    def xml = msg.xml                // => any XML included in response body, as a document tree structure
    def data = msg.data              // => either JSON or XML in response body (whichever is specified by content-type header in response)
        
    log.debug "json: " + json.toString()
        
    // createEvent returns a Map that defines an Event
    def result = createEvent(name: "alarmStatus", value: json)
    
    log.debug "Parse returned ${result?.descriptionText}"
    
    // returning the Event definition map creates an Event
    // in the SmartThings platform, and propagates it to
    // SmartApps subscribed to the device events.
    return result
}

// handle commands
/*
def off() {
	log.debug "Executing 'off'"
	// TODO: handle 'off' command
}

def strobe() {
	log.debug "Executing 'strobe'"
	// TODO: handle 'strobe' command
}

def siren() {
	log.debug "Executing 'siren'"
	// TODO: handle 'siren' command
}

def both() {
	log.debug "Executing 'both'"
	// TODO: handle 'both' command
}
*/
/*
def sendEvent(alarmSystemStatus,off)() {
	log.debug "Executing 'sendEvent(alarmSystemStatus,off)'"
	// TODO: handle 'sendEvent(alarmSystemStatus,off)' command
}

def sendEvent(alarmSystemStatus,stay)() {
	log.debug "Executing 'sendEvent(alarmSystemStatus,stay)'"
	// TODO: handle 'sendEvent(alarmSystemStatus,stay)' command
}

def sendEvent(alarmSystemStatus,away)() {
	log.debug "Executing 'sendEvent(alarmSystemStatus,away)'"
	// TODO: handle 'sendEvent(alarmSystemStatus,away)' command
}
*/

def configure() {
	log.debug "Executing 'configure'"
    initialize()
}

/*
def poll() {
	log.debug "Executing 'poll'"
	// TODO: handle 'poll' command
    myCommand()
}
*/
def refresh() {
	log.debug "Executing 'refresh'"
    getAlarmStatus()
    
    /*
    // To the parse() function get called, no callback function must be present, and the deviceNetworkId must be UPPERCASE and
    // set to MAC address (without colons) or IP address:port (HEX IP : 4 digits HEX port) must be used (ex: C0A8020A:0253).
    def action = new physicalgraph.device.HubAction([
        method: "GET",
        path: "/Alarm/Status",
        headers: [
            HOST: getHostAddress()
        ]]
        ,device.deviceNetworkId // Optional
    )

	return action
    */
}

def armStay() {
	log.debug "Executing 'armStay'"
    postAlarmOperation("/Alarm/Arm/stay")    
    sendEvent(name: "alarmSystemStatus", value: 'stay')
}

def armAway() {
	log.debug "Executing 'armAway'"
    postAlarmOperation("/Alarm/Arm/away")    
    sendEvent(name: "alarmSystemStatus", value: 'away')
}

def disarm() {
	log.debug "Executing 'disarm'"
    postAlarmOperation("/Alarm/Arm/off")
    sendEvent(name: "alarmSystemStatus", value: 'off')
}

def chimeOn() {
	log.debug "Executing 'chimeOn'"
    postAlarmOperation("/Alarm/Chime")
    sendEvent(name: "isChimeEnabled", value: true)
}

def chimeOff() {
	log.debug "Executing 'chimeOff'"
    postAlarmOperation("/Alarm/Chime")
    sendEvent(name: "isChimeEnabled", value: false)
}

def getAlarmStatus() {
	try {
        /*def map = [ eventid: "${evt.id}",
              date: "${evt.isoDate}",
              eventname: "${evt.name}",
              devicename: "${evt.displayName}",
              value: "${evt.stringValue}"
              ]
 
    	def json = new groovy.json.JsonBuilder(map).toString()*/

        def action = new physicalgraph.device.HubAction([
            method: "GET",
            path: "/Alarm/Status",
            headers: [
                HOST: getHostAddress()
            ]]
           	,device.deviceNetworkId
           	,[callback: "alarmStatusHandler"]
        	//query: [param1: "value1", param2: "value2"]
        )
        
        log.debug "deviceNetworkId: " + device.deviceNetworkId
        log.debug "Result: " + action.toString()
        
		sendHubCommand(action)
    } catch (e) {
    	log.debug(e.message)
    }
}

void alarmStatusHandler(physicalgraph.device.HubResponse hubResponse) {
    log.debug "Entered alarmStatusHandler()..."

/*
    log.debug "hubResponse: " + hubResponse
    log.debug "body: " + hubResponse.body
    log.debug "data: " + hubResponse.data
    log.debug "description: " + hubResponse.description
    log.debug "error: " + hubResponse.error
    log.debug "header: " + hubResponse.header
    log.debug "headers: " + hubResponse.headers
    log.debug "hubId: " + hubResponse.hubId
    log.debug "json: " + hubResponse.json
    log.debug "status: " + hubResponse.status
    log.debug "xml: " + hubResponse.xml
*/

	sendAlarmStatusEvents(hubResponse.json)

/*
	if (hubResponse.json) {
    	log.debug "json ok"
        
        sendEvent(name: "alarmStatus", value: hubResponse.json)
        sendEvent(name: "alarmSystemStatus", value: hubResponse.json.armType)
        sendEvent(name: "securitySystemStatus", value: hubResponse.json.armType)
        sendEvent(name: "isSystemReady", value: hubResponse.json.isSystemReady)
        sendEvent(name: "isFireAlarm", value: hubResponse.json.isFireAlarm)
        sendEvent(name: "isIntrusionAlarm", value: hubResponse.json.isIntrusionAlarm)
        sendEvent(name: "isExitTimeDelay", value: hubResponse.json.isExitTimeDelay)
        sendEvent(name: "isEntryTimeDelay", value: hubResponse.json.isEntryTimeDelay)
        sendEvent(name: "isZoneBypassEnabled", value: hubResponse.json.isZoneBypassEnabled)
        sendEvent(name: "isChimeEnabled", value: hubResponse.json.isChimeEnabled)
        sendEvent(name: "systemStatus", value: hubResponse.json.systemStatus)
        sendEvent(name: "zones", value: hubResponse.json.zones)
       
        if (hubResponse.json.isFireAlarm || hubResponse.json.isIntrusionAlarm)
        {
        	sendEvent(name: "alarm", value: true)
        } else {
        	sendEvent(name: "alarm", value: false)
        }
	}
*/
}

def postAlarmOperation(path) {
	log.debug "postAlarmOperation '${path}'"

	try {
        def action = new physicalgraph.device.HubAction([
            method: "POST",
            path: path,
            headers: [
                HOST: getHostAddress()
            ]]
           	,device.deviceNetworkId
           	,[callback: "alarmOperationHandler"]
        )
        
        log.debug "deviceNetworkId: " + device.deviceNetworkId
        log.debug "Result: " + action.toString()
        
		sendHubCommand(action)
    } catch (e) {
    	log.debug(e.message)
    }
}

void alarmOperationHandler(physicalgraph.device.HubResponse hubResponse) {
    log.debug "Entered alarmOperationHandler()..."
    
  log.debug "hubResponse: " + hubResponse
    log.debug "body: " + hubResponse.body
    log.debug "data: " + hubResponse.data
    log.debug "description: " + hubResponse.description
    log.debug "error: " + hubResponse.error
    log.debug "header: " + hubResponse.header
    log.debug "headers: " + hubResponse.headers
    log.debug "hubId: " + hubResponse.hubId
    //log.debug "json: " + hubResponse.json
    log.debug "status: " + hubResponse.status
    log.debug "xml: " + hubResponse.xml

	sendAlarmStatusEvents(hubResponse.json)

/*	if (hubResponse.json) {
    	log.debug "json ok"
        
        sendEvent(name: "alarmStatus", value: hubResponse.json)
        sendEvent(name: "alarmSystemStatus", value: hubResponse.json.armType)
        sendEvent(name: "securitySystemStatus", value: hubResponse.json.armType)
        sendEvent(name: "isSystemReady", value: hubResponse.json.isSystemReady)
        sendEvent(name: "isFireAlarm", value: hubResponse.json.isFireAlarm)
        sendEvent(name: "isIntrusionAlarm", value: hubResponse.json.isIntrusionAlarm)
        sendEvent(name: "isExitTimeDelay", value: hubResponse.json.isExitTimeDelay)
        sendEvent(name: "isEntryTimeDelay", value: hubResponse.json.isEntryTimeDelay)
        sendEvent(name: "isZoneBypassEnabled", value: hubResponse.json.isZoneBypassEnabled)
        sendEvent(name: "isChimeEnabled", value: hubResponse.json.isChimeEnabled)
        sendEvent(name: "systemStatus", value: hubResponse.json.systemStatus)
        sendEvent(name: "zones", value: hubResponse.json.zones)
       
        if (hubResponse.json.isFireAlarm || hubResponse.json.isIntrusionAlarm)
        {
        	sendEvent(name: "alarm", value: true)
        } else {
        	sendEvent(name: "alarm", value: false)
        }
    }
*/
}

def sendAlarmStatusEvents(jsonStatus) {
 	log.debug "Entered sendAlarmStatusEvents()..."
    log.debug "jsonStatus: " + jsonStatus
    
	 if (jsonStatus) {
    	log.debug "jsonStatus ok"
        
        sendEvent(name: "alarmStatus", value: jsonStatus)
        sendEvent(name: "alarmSystemStatus", value: jsonStatus.armType)
        sendEvent(name: "securitySystemStatus", value: jsonStatus.armType)
        sendEvent(name: "isSystemReady", value: jsonStatus.isSystemReady)
        sendEvent(name: "isFireAlarm", value: jsonStatus.isFireAlarm)
        sendEvent(name: "isIntrusionAlarm", value: jsonStatus.isIntrusionAlarm)
        sendEvent(name: "isExitTimeDelay", value: jsonStatus.isExitTimeDelay)
        sendEvent(name: "isEntryTimeDelay", value: jsonStatus.isEntryTimeDelay)
        sendEvent(name: "isZoneBypassEnabled", value: jsonStatus.isZoneBypassEnabled)
        sendEvent(name: "isChimeEnabled", value: jsonStatus.isChimeEnabled)
        sendEvent(name: "systemStatus", value: jsonStatus.systemStatus)
        sendEvent(name: "zones", value: jsonStatus.zones)
        
        if (jsonStatus.isFireAlarm || jsonStatus.isIntrusionAlarm)
        {
        	sendEvent(name: "alarm", value: true)
        } else {
        	sendEvent(name: "alarm", value: false)
        }
    }
}

//
// Helper methods
//

// gets the address of the Hub
private getCallBackAddress() {
    return device.hub.getDataValue("localIP") + ":" + device.hub.getDataValue("localSrvPortTCP")
}

// gets the address of the device
private getHostAddress() {
    if (ipAddress && portNumber)
    {
    	log.debug "Using IP: $ipAddress and port: $portNumber from preferences"
        
        return  ipAddress + ":" + portNumber
	} else {
        def ip = getDataValue("ip")
        def port = getDataValue("port")

        if (!ip || !port) {
            def parts = device.deviceNetworkId.split(":")
            if (parts.length == 2) {
                ip = parts[0]
                port = parts[1]
            } else {
                log.warn "Can't figure out ip and port for device: ${device.id}"
            }
        }

        log.debug "Using IP: $ip and port: $port for device: ${device.id}"
        
        def ipConv = convertHexToIP(ip)
        def portConv = convertHexToInt(port)
        
        log.debug "Converted IP: $ipConv and port: $portConv"

        return  ipConv + ":" + portConv
    }
}

private Integer convertHexToInt(hex) {
    return Integer.parseInt(hex,16)
}

private String convertHexToIP(hex) {
    return [convertHexToInt(hex[0..1]),convertHexToInt(hex[2..3]),convertHexToInt(hex[4..5]),convertHexToInt(hex[6..7])].join(".")
}
