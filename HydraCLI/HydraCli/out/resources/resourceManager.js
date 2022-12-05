"use strict";
/**
 * This class gives you type-hinting for the automatic generated resx-json files
 */
Object.defineProperty(exports, "__esModule", { value: true });
exports.HydraCli = void 0;
class resourceManager {
    language;
    constructor(language) {
        this.language = language;
    }
    setLanguage(language) {
        this.language = language;
    }
    ;
    // Generated class instances start
    _HydraCli = new HydraCli(this);
    get HydraCli() {
        return this._HydraCli;
    }
}
exports.default = resourceManager;
class resourceFile {
    resMan;
    resources = {};
    constructor(resourceManager) {
        this.resMan = resourceManager;
    }
    get(resKey) {
        let language = this.resMan.language;
        // Check if the language exists for this resource and if the language has an corresponsing key
        if (this.resources.hasOwnProperty(language) && this.resources[language].hasOwnProperty(resKey)) {
            return this.resources[language][resKey];
        }
        // If no entry could be found in the currently active language, try the default language
        if (this.resources.hasOwnProperty('HydraCli.en-US.resx') && this.resources['HydraCli.en-US.resx'].hasOwnProperty(resKey)) {
            console.log(`No text resource in the language "${language}" with the key "${resKey}".`);
            return this.resources['HydraCli.en-US.resx'][resKey];
        }
        // If there is still no resource found output a warning and return the key.
        console.warn(`No text-resource for the key ${resKey} found.`);
        return resKey;
    }
    ;
}
// Gen Classes start
const resxHydraCli = require("./HydraCli.json");
class HydraCli extends resourceFile {
    constructor(resourceManager) {
        super(resourceManager);
        this.resources = resxHydraCli;
    }
    get The_starter_template_file_for_either3() {
        return this.get('The_starter_template_file_for_either3');
    }
    get to_run_this_command() {
        return this.get('to_run_this_command');
    }
    get For_help_getting_started_visit() {
        return this.get('For_help_getting_started_visit');
    }
    get Initializing_renderer_Starting4() {
        return this.get('Initializing_renderer_Starting4');
    }
    get Error_Not_erasing_existing_project() {
        return this.get('Error_Not_erasing_existing_project');
    }
    get Method_not_implemented() {
        return this.get('Method_not_implemented');
    }
    get Initializing_renderer_Starting3() {
        return this.get('Initializing_renderer_Starting3');
    }
    get Saves_client_processing_in_the_Logs() {
        return this.get('Saves_client_processing_in_the_Logs');
    }
    get Finished_creating_project() {
        return this.get('Finished_creating_project');
    }
    get The_json_input_file_for_processing_the5() {
        return this.get('The_json_input_file_for_processing_the5');
    }
    get The_json_input_file_for_processing_the4() {
        return this.get('The_json_input_file_for_processing_the4');
    }
    get Did_you_run_hydra_start() {
        return this.get('Did_you_run_hydra_start');
    }
    get The_json_input_file_for_processing_the1() {
        return this.get('The_json_input_file_for_processing_the1');
    }
    get The_json_input_file_for_processing_the3() {
        return this.get('The_json_input_file_for_processing_the3');
    }
    get The_json_input_file_for_processing_the2() {
        return this.get('The_json_input_file_for_processing_the2');
    }
    get getpackagedevinstalls() {
        return this.get('getpackagedevinstalls');
    }
    get No_packages_found() {
        return this.get('No_packages_found');
    }
    get Downloading_template() {
        return this.get('Downloading_template');
    }
    get The_json_input_file_for_processing_the() {
        return this.get('The_json_input_file_for_processing_the');
    }
    get Starting_template_generation() {
        return this.get('Starting_template_generation');
    }
    get Signalling_agent_to_end_processing() {
        return this.get('Signalling_agent_to_end_processing');
    }
    get Launching_headless_browser5() {
        return this.get('Launching_headless_browser5');
    }
    get Generates_either_an_entities_json() {
        return this.get('Generates_either_an_entities_json');
    }
    get Unexpected_incompletion_of() {
        return this.get('Unexpected_incompletion_of');
    }
    get Copyright_CloudIDEaaS() {
        return this.get('Copyright_CloudIDEaaS');
    }
    get Launching_headless_browser() {
        return this.get('Launching_headless_browser');
    }
    get Package_dev_installs_started() {
        return this.get('Package_dev_installs_started');
    }
    get Thank_you_for_installing_Hydra() {
        return this.get('Thank_you_for_installing_Hydra');
    }
    get Waiting_for_json_command() {
        return this.get('Waiting_for_json_command');
    }
    get Application_not_fully_installed() {
        return this.get('Application_not_fully_installed');
    }
    get The_application_name() {
        return this.get('The_application_name');
    }
    get Signalling_agent_to_close() {
        return this.get('Signalling_agent_to_close');
    }
    get ApplicationGeneratorAgent_process() {
        return this.get('ApplicationGeneratorAgent_process');
    }
    get read_package_json() {
        return this.get('read_package_json');
    }
    get Skips_installing_Ionic() {
        return this.get('Skips_installing_Ionic');
    }
    get The_organization_name3() {
        return this.get('The_organization_name3');
    }
    get Adds_resource_to_be_used_later_in_the() {
        return this.get('Adds_resource_to_be_used_later_in_the');
    }
    get For_help_getting_started_visit1() {
        return this.get('For_help_getting_started_visit1');
    }
    get For_help_getting_started_visit3() {
        return this.get('For_help_getting_started_visit3');
    }
    get For_help_getting_started_visit2() {
        return this.get('For_help_getting_started_visit2');
    }
    get For_help_getting_started_visit5() {
        return this.get('For_help_getting_started_visit5');
    }
    get For_help_getting_started_visit4() {
        return this.get('For_help_getting_started_visit4');
    }
    get Pauses_to_allow_for_debug_attach() {
        return this.get('Pauses_to_allow_for_debug_attach');
    }
    get EndOfProcessing() {
        return this.get('EndOfProcessing');
    }
    get The_target_for_the_generate_command() {
        return this.get('The_target_for_the_generate_command');
    }
    get Installs_package() {
        return this.get('Installs_package');
    }
    get The_organization_name() {
        return this.get('The_organization_name');
    }
    get Generates_business_model() {
        return this.get('Generates_business_model');
    }
    get Launching_Cache_Server() {
        return this.get('Launching_Cache_Server');
    }
    get Generates_workspace_for_NET_Visual() {
        return this.get('Generates_workspace_for_NET_Visual');
    }
    get ionic_cordova_platform_remove() {
        return this.get('ionic_cordova_platform_remove');
    }
    get Agent_closed_successfully() {
        return this.get('Agent_closed_successfully');
    }
    get Updates_platform_in_the_front_end_app() {
        return this.get('Updates_platform_in_the_front_end_app');
    }
    get The_appplication_description1() {
        return this.get('The_appplication_description1');
    }
    get Saves_restful_service_messages_in_the() {
        return this.get('Saves_restful_service_messages_in_the');
    }
    get Package_cache_processing_may_take() {
        return this.get('Package_cache_processing_may_take');
    }
    get Go_to_your_cloned_project() {
        return this.get('Go_to_your_cloned_project');
    }
    get The_appplication_description() {
        return this.get('The_appplication_description');
    }
    get The_starter_template_file_for_either2() {
        return this.get('The_starter_template_file_for_either2');
    }
    get Finished_creating_project1() {
        return this.get('Finished_creating_project1');
    }
    get Generates_fully_funtional() {
        return this.get('Generates_fully_funtional');
    }
    get package_json_does_not_include() {
        return this.get('package_json_does_not_include');
    }
    get Launching_headless_browser3() {
        return this.get('Launching_headless_browser3');
    }
    get The_organization_name1() {
        return this.get('The_organization_name1');
    }
    get Launching_headless_browser1() {
        return this.get('Launching_headless_browser1');
    }
    get The_starter_template_file_for_either() {
        return this.get('The_starter_template_file_for_either');
    }
    get List_of_packages_in_comma_delimited() {
        return this.get('List_of_packages_in_comma_delimited');
    }
    get Generation_completed() {
        return this.get('Generation_completed');
    }
    get Thank_you_for_installing_Hydra1() {
        return this.get('Thank_you_for_installing_Hydra1');
    }
    get Thank_you_for_installing_Hydra3() {
        return this.get('Thank_you_for_installing_Hydra3');
    }
    get Finished_installing_the_Ionic() {
        return this.get('Finished_installing_the_Ionic');
    }
    get The_business_model_json_input_file_for4() {
        return this.get('The_business_model_json_input_file_for4');
    }
    get The_business_model_json_input_file_for3() {
        return this.get('The_business_model_json_input_file_for3');
    }
    get The_business_model_json_input_file_for2() {
        return this.get('The_business_model_json_input_file_for2');
    }
    get The_business_model_json_input_file_for1() {
        return this.get('The_business_model_json_input_file_for1');
    }
    get Launches_and_serves_the_front_end_app() {
        return this.get('Launches_and_serves_the_front_end_app');
    }
    get Cache_server_shut_down() {
        return this.get('Cache_server_shut_down');
    }
    get The_business_model_json_input_file_for5() {
        return this.get('The_business_model_json_input_file_for5');
    }
    get ionic_cordova_platform_update() {
        return this.get('ionic_cordova_platform_update');
    }
    get connected_to_parent_service() {
        return this.get('connected_to_parent_service');
    }
    get Not_erasing_existing_project() {
        return this.get('Not_erasing_existing_project');
    }
    get Thank_you_for_installing_Hydra2() {
        return this.get('Thank_you_for_installing_Hydra2');
    }
    get Thank_you_for_installing_Hydra5() {
        return this.get('Thank_you_for_installing_Hydra5');
    }
    get Thank_you_for_installing_Hydra4() {
        return this.get('Thank_you_for_installing_Hydra4');
    }
    get Hydra_is_an_application_generation() {
        return this.get('Hydra_is_an_application_generation');
    }
    get You_must_install() {
        return this.get('You_must_install');
    }
    get installsComplete() {
        return this.get('installsComplete');
    }
    get ionic_cordova_platform_add() {
        return this.get('ionic_cordova_platform_add');
    }
    get package_json_does_not_include1() {
        return this.get('package_json_does_not_include1');
    }
    get The_organization_name4() {
        return this.get('The_organization_name4');
    }
    get The_business_model_json_input_file_for() {
        return this.get('The_business_model_json_input_file_for');
    }
    get The_organization_name5() {
        return this.get('The_organization_name5');
    }
    get Launching_headless_browser2() {
        return this.get('Launching_headless_browser2');
    }
    get Next_status_update() {
        return this.get('Next_status_update');
    }
    get This_is_test_Nothing_will_actually_be() {
        return this.get('This_is_test_Nothing_will_actually_be');
    }
    get Invoking_Hydra() {
        return this.get('Invoking_Hydra');
    }
    get Connected_successfully() {
        return this.get('Connected_successfully');
    }
    get There_was_an_error_reading_the_package() {
        return this.get('There_was_an_error_reading_the_package');
    }
    get Installing_the_Ionic_Framework() {
        return this.get('Installing_the_Ionic_Framework');
    }
    get The_organization_name2() {
        return this.get('The_organization_name2');
    }
    get Installing_dependencies_may_take() {
        return this.get('Installing_dependencies_may_take');
    }
    get Initializing_renderer_Starting() {
        return this.get('Initializing_renderer_Starting');
    }
    get ionic_cordova_platform_ls() {
        return this.get('ionic_cordova_platform_ls');
    }
    get No_writeStatus_output_provided() {
        return this.get('No_writeStatus_output_provided');
    }
    get Initializing_renderer_Starting5() {
        return this.get('Initializing_renderer_Starting5');
    }
    get Runs_command_without_actually() {
        return this.get('Runs_command_without_actually');
    }
    get The_starter_template_file_for_either5() {
        return this.get('The_starter_template_file_for_either5');
    }
    get The_starter_template_file_for_either4() {
        return this.get('The_starter_template_file_for_either4');
    }
    get Initializing_renderer_Starting1() {
        return this.get('Initializing_renderer_Starting1');
    }
    get Launching_headless_browser4() {
        return this.get('Launching_headless_browser4');
    }
    get The_starter_template_file_for_either1() {
        return this.get('The_starter_template_file_for_either1');
    }
    get Initializing_renderer_Starting2() {
        return this.get('Initializing_renderer_Starting2');
    }
}
exports.HydraCli = HydraCli;
// Gen Classes end
//# sourceMappingURL=resourceManager.js.map