
/**
 * This class gives you type-hinting for the automatic generated resx-json files
 */

export default class resourceManager {

    public language: string;

    constructor(language: string) {
        this.language = language;
    }

    public setLanguage(language: string) {
        this.language = language;
    };

    // Generated class instances start
    
    private _HydraCli: HydraCli = new HydraCli(this);
    get HydraCli(): HydraCli {
        return this._HydraCli;
    }
    
    // Gen end
}

abstract class resourceFile {
    protected resMan: resourceManager;
    protected resources: { [langKey: string]: { [resKey: string]: string } } = {};

    constructor(resourceManager: resourceManager) {
        this.resMan = resourceManager;
    }

    public get(resKey: string) {
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
    };
}

// Gen Classes start

import * as resxHydraCli from './HydraCli.json';

export class HydraCli extends resourceFile {

    constructor(resourceManager: resourceManager) {
        super(resourceManager);
        this.resources = (<any>resxHydraCli);
    }
    
    get The_starter_template_file_for_either3(): string {
        return this.get('The_starter_template_file_for_either3');
    }
    
    get to_run_this_command(): string {
        return this.get('to_run_this_command');
    }
    
    get For_help_getting_started_visit(): string {
        return this.get('For_help_getting_started_visit');
    }
    
    get Initializing_renderer_Starting4(): string {
        return this.get('Initializing_renderer_Starting4');
    }
    
    get Error_Not_erasing_existing_project(): string {
        return this.get('Error_Not_erasing_existing_project');
    }
    
    get Method_not_implemented(): string {
        return this.get('Method_not_implemented');
    }
    
    get Initializing_renderer_Starting3(): string {
        return this.get('Initializing_renderer_Starting3');
    }
    
    get Saves_client_processing_in_the_Logs(): string {
        return this.get('Saves_client_processing_in_the_Logs');
    }
    
    get Finished_creating_project(): string {
        return this.get('Finished_creating_project');
    }
    
    get The_json_input_file_for_processing_the5(): string {
        return this.get('The_json_input_file_for_processing_the5');
    }
    
    get The_json_input_file_for_processing_the4(): string {
        return this.get('The_json_input_file_for_processing_the4');
    }
    
    get Did_you_run_hydra_start(): string {
        return this.get('Did_you_run_hydra_start');
    }
    
    get The_json_input_file_for_processing_the1(): string {
        return this.get('The_json_input_file_for_processing_the1');
    }
    
    get The_json_input_file_for_processing_the3(): string {
        return this.get('The_json_input_file_for_processing_the3');
    }
    
    get The_json_input_file_for_processing_the2(): string {
        return this.get('The_json_input_file_for_processing_the2');
    }
    
    get getpackagedevinstalls(): string {
        return this.get('getpackagedevinstalls');
    }
    
    get No_packages_found(): string {
        return this.get('No_packages_found');
    }
    
    get Downloading_template(): string {
        return this.get('Downloading_template');
    }
    
    get The_json_input_file_for_processing_the(): string {
        return this.get('The_json_input_file_for_processing_the');
    }
    
    get Starting_template_generation(): string {
        return this.get('Starting_template_generation');
    }
    
    get Signalling_agent_to_end_processing(): string {
        return this.get('Signalling_agent_to_end_processing');
    }
    
    get Launching_headless_browser5(): string {
        return this.get('Launching_headless_browser5');
    }
    
    get Generates_either_an_entities_json(): string {
        return this.get('Generates_either_an_entities_json');
    }
    
    get Unexpected_incompletion_of(): string {
        return this.get('Unexpected_incompletion_of');
    }
    
    get Copyright_CloudIDEaaS(): string {
        return this.get('Copyright_CloudIDEaaS');
    }
    
    get Launching_headless_browser(): string {
        return this.get('Launching_headless_browser');
    }
    
    get Package_dev_installs_started(): string {
        return this.get('Package_dev_installs_started');
    }
    
    get Thank_you_for_installing_Hydra(): string {
        return this.get('Thank_you_for_installing_Hydra');
    }
    
    get Waiting_for_json_command(): string {
        return this.get('Waiting_for_json_command');
    }
    
    get Application_not_fully_installed(): string {
        return this.get('Application_not_fully_installed');
    }
    
    get The_application_name(): string {
        return this.get('The_application_name');
    }
    
    get Signalling_agent_to_close(): string {
        return this.get('Signalling_agent_to_close');
    }
    
    get ApplicationGeneratorAgent_process(): string {
        return this.get('ApplicationGeneratorAgent_process');
    }
    
    get read_package_json(): string {
        return this.get('read_package_json');
    }
    
    get Skips_installing_Ionic(): string {
        return this.get('Skips_installing_Ionic');
    }
    
    get The_organization_name3(): string {
        return this.get('The_organization_name3');
    }
    
    get Adds_resource_to_be_used_later_in_the(): string {
        return this.get('Adds_resource_to_be_used_later_in_the');
    }
    
    get For_help_getting_started_visit1(): string {
        return this.get('For_help_getting_started_visit1');
    }
    
    get For_help_getting_started_visit3(): string {
        return this.get('For_help_getting_started_visit3');
    }
    
    get For_help_getting_started_visit2(): string {
        return this.get('For_help_getting_started_visit2');
    }
    
    get For_help_getting_started_visit5(): string {
        return this.get('For_help_getting_started_visit5');
    }
    
    get For_help_getting_started_visit4(): string {
        return this.get('For_help_getting_started_visit4');
    }
    
    get Pauses_to_allow_for_debug_attach(): string {
        return this.get('Pauses_to_allow_for_debug_attach');
    }
    
    get EndOfProcessing(): string {
        return this.get('EndOfProcessing');
    }
    
    get The_target_for_the_generate_command(): string {
        return this.get('The_target_for_the_generate_command');
    }
    
    get Installs_package(): string {
        return this.get('Installs_package');
    }
    
    get The_organization_name(): string {
        return this.get('The_organization_name');
    }
    
    get Generates_business_model(): string {
        return this.get('Generates_business_model');
    }
    
    get Launching_Cache_Server(): string {
        return this.get('Launching_Cache_Server');
    }
    
    get Generates_workspace_for_NET_Visual(): string {
        return this.get('Generates_workspace_for_NET_Visual');
    }
    
    get ionic_cordova_platform_remove(): string {
        return this.get('ionic_cordova_platform_remove');
    }
    
    get Agent_closed_successfully(): string {
        return this.get('Agent_closed_successfully');
    }
    
    get Updates_platform_in_the_front_end_app(): string {
        return this.get('Updates_platform_in_the_front_end_app');
    }
    
    get The_appplication_description1(): string {
        return this.get('The_appplication_description1');
    }
    
    get Saves_restful_service_messages_in_the(): string {
        return this.get('Saves_restful_service_messages_in_the');
    }
    
    get Package_cache_processing_may_take(): string {
        return this.get('Package_cache_processing_may_take');
    }
    
    get Go_to_your_cloned_project(): string {
        return this.get('Go_to_your_cloned_project');
    }
    
    get The_appplication_description(): string {
        return this.get('The_appplication_description');
    }
    
    get The_starter_template_file_for_either2(): string {
        return this.get('The_starter_template_file_for_either2');
    }
    
    get Finished_creating_project1(): string {
        return this.get('Finished_creating_project1');
    }
    
    get Generates_fully_funtional(): string {
        return this.get('Generates_fully_funtional');
    }
    
    get package_json_does_not_include(): string {
        return this.get('package_json_does_not_include');
    }
    
    get Launching_headless_browser3(): string {
        return this.get('Launching_headless_browser3');
    }
    
    get The_organization_name1(): string {
        return this.get('The_organization_name1');
    }
    
    get Launching_headless_browser1(): string {
        return this.get('Launching_headless_browser1');
    }
    
    get The_starter_template_file_for_either(): string {
        return this.get('The_starter_template_file_for_either');
    }
    
    get List_of_packages_in_comma_delimited(): string {
        return this.get('List_of_packages_in_comma_delimited');
    }
    
    get Generation_completed(): string {
        return this.get('Generation_completed');
    }
    
    get Thank_you_for_installing_Hydra1(): string {
        return this.get('Thank_you_for_installing_Hydra1');
    }
    
    get Thank_you_for_installing_Hydra3(): string {
        return this.get('Thank_you_for_installing_Hydra3');
    }
    
    get Finished_installing_the_Ionic(): string {
        return this.get('Finished_installing_the_Ionic');
    }
    
    get The_business_model_json_input_file_for4(): string {
        return this.get('The_business_model_json_input_file_for4');
    }
    
    get The_business_model_json_input_file_for3(): string {
        return this.get('The_business_model_json_input_file_for3');
    }
    
    get The_business_model_json_input_file_for2(): string {
        return this.get('The_business_model_json_input_file_for2');
    }
    
    get The_business_model_json_input_file_for1(): string {
        return this.get('The_business_model_json_input_file_for1');
    }
    
    get Launches_and_serves_the_front_end_app(): string {
        return this.get('Launches_and_serves_the_front_end_app');
    }
    
    get Cache_server_shut_down(): string {
        return this.get('Cache_server_shut_down');
    }
    
    get The_business_model_json_input_file_for5(): string {
        return this.get('The_business_model_json_input_file_for5');
    }
    
    get ionic_cordova_platform_update(): string {
        return this.get('ionic_cordova_platform_update');
    }
    
    get connected_to_parent_service(): string {
        return this.get('connected_to_parent_service');
    }
    
    get Not_erasing_existing_project(): string {
        return this.get('Not_erasing_existing_project');
    }
    
    get Thank_you_for_installing_Hydra2(): string {
        return this.get('Thank_you_for_installing_Hydra2');
    }
    
    get Thank_you_for_installing_Hydra5(): string {
        return this.get('Thank_you_for_installing_Hydra5');
    }
    
    get Thank_you_for_installing_Hydra4(): string {
        return this.get('Thank_you_for_installing_Hydra4');
    }
    
    get Hydra_is_an_application_generation(): string {
        return this.get('Hydra_is_an_application_generation');
    }
    
    get You_must_install(): string {
        return this.get('You_must_install');
    }
    
    get installsComplete(): string {
        return this.get('installsComplete');
    }
    
    get ionic_cordova_platform_add(): string {
        return this.get('ionic_cordova_platform_add');
    }
    
    get package_json_does_not_include1(): string {
        return this.get('package_json_does_not_include1');
    }
    
    get The_organization_name4(): string {
        return this.get('The_organization_name4');
    }
    
    get The_business_model_json_input_file_for(): string {
        return this.get('The_business_model_json_input_file_for');
    }
    
    get The_organization_name5(): string {
        return this.get('The_organization_name5');
    }
    
    get Launching_headless_browser2(): string {
        return this.get('Launching_headless_browser2');
    }
    
    get Next_status_update(): string {
        return this.get('Next_status_update');
    }
    
    get This_is_test_Nothing_will_actually_be(): string {
        return this.get('This_is_test_Nothing_will_actually_be');
    }
    
    get Invoking_Hydra(): string {
        return this.get('Invoking_Hydra');
    }
    
    get Connected_successfully(): string {
        return this.get('Connected_successfully');
    }
    
    get There_was_an_error_reading_the_package(): string {
        return this.get('There_was_an_error_reading_the_package');
    }
    
    get Installing_the_Ionic_Framework(): string {
        return this.get('Installing_the_Ionic_Framework');
    }
    
    get The_organization_name2(): string {
        return this.get('The_organization_name2');
    }
    
    get Installing_dependencies_may_take(): string {
        return this.get('Installing_dependencies_may_take');
    }
    
    get Initializing_renderer_Starting(): string {
        return this.get('Initializing_renderer_Starting');
    }
    
    get ionic_cordova_platform_ls(): string {
        return this.get('ionic_cordova_platform_ls');
    }
    
    get No_writeStatus_output_provided(): string {
        return this.get('No_writeStatus_output_provided');
    }
    
    get Initializing_renderer_Starting5(): string {
        return this.get('Initializing_renderer_Starting5');
    }
    
    get Runs_command_without_actually(): string {
        return this.get('Runs_command_without_actually');
    }
    
    get The_starter_template_file_for_either5(): string {
        return this.get('The_starter_template_file_for_either5');
    }
    
    get The_starter_template_file_for_either4(): string {
        return this.get('The_starter_template_file_for_either4');
    }
    
    get Initializing_renderer_Starting1(): string {
        return this.get('Initializing_renderer_Starting1');
    }
    
    get Launching_headless_browser4(): string {
        return this.get('Launching_headless_browser4');
    }
    
    get The_starter_template_file_for_either1(): string {
        return this.get('The_starter_template_file_for_either1');
    }
    
    get Initializing_renderer_Starting2(): string {
        return this.get('Initializing_renderer_Starting2');
    }
    
}
 
// Gen Classes end
